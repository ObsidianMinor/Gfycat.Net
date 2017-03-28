using Gfycat.API.Models;
using Gfycat.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gfycat.API
{
    internal class GfycatApiClient
    {
        internal IRestClient RestClient => Config.RestClient;
        internal GfycatClient Client { get; }
        internal GfycatClientConfig Config { get; }

        internal GfycatApiClient(GfycatClient client, GfycatClientConfig config)
        {
            Client = client;
            Config = config;
        }

        #region Rest Client stuff

        internal async Task<RestResponse> SendAsync(string method, string endpoint, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            return await SendInternalAsync(() => RestClient.SendAsync(method, endpoint, options.CancellationToken), options).ConfigureAwait(false);
        }

        internal async Task<RestResponse> SendJsonAsync(string method, string endpoint, object jsonObject, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            string jsonString = JsonConvert.SerializeObject(jsonObject, Config.SerializerSettings);
            return await SendInternalAsync(() => RestClient.SendAsync(method, endpoint, jsonString, options.CancellationToken), options).ConfigureAwait(false);
        }

        internal async Task<RestResponse> SendStreamAsync(string method, string endpoint, Stream stream, string fileName, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            Dictionary<string, object> content = new Dictionary<string, object>
            {
                { "file", new MultipartFile(stream, fileName) }
            };
            return await SendInternalAsync(() => RestClient.SendAsync(method, endpoint, content, options.CancellationToken), options).ConfigureAwait(false);
        }

        private async Task<RestResponse> SendBinaryStreamAsync(string method, string endpoint, Stream stream, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            return await SendInternalAsync(() => RestClient.SendAsync(method, endpoint, stream, options.CancellationToken), options).ConfigureAwait(false);
        }

        private async Task<RestResponse> SendInternalAsync(Func<Task<RestResponse>> RestFunction, RequestOptions options)
        {
            RestResponse response = null;
            RestClient.SetHeader("Authorization", options.UseAccessToken ? $"Bearer {Client.AccessToken}" : null);
            bool retry = true, first401 = true;
            while (retry)
            {
                Task<RestResponse> taskResponse = RestFunction();
                bool taskTimedout = !taskResponse.Wait(options.Timeout ?? -1);
                if (taskTimedout && options.RetryMode != RetryMode.RetryTimeouts) // the blocking wait call will complete the task, so in the following checks we can just access the result normally
                    throw new TimeoutException($"The request timed out");
                else if (taskResponse.Result.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502))
                    throw GfycatException.CreateFromResponse(taskResponse.Result);
                else if (taskResponse.Result.Status == HttpStatusCode.Unauthorized)
                    if (options.UseAccessToken && (first401 && options.RetryMode.HasFlag(RetryMode.RetryFirst401))) // make sure we don't get in a refresh loop due to not having an access token when using an invalid client ID
                    {
                        await Client.RefreshTokenAsync();
                        first401 = false;
                    }
                    else
                        throw GfycatException.CreateFromResponse(taskResponse.Result);
                else
                {
                    response = taskResponse.Result;
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code == response.Status) ?? false))
                    {
                        throw GfycatException.CreateFromResponse(response);
                    }
                    retry = false;
                }
            }

            return response;
        }

        #endregion

        #region Users

        internal async Task<HttpStatusCode> GetUsernameStatusAsync(string username, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("HEAD", $"users/{username}", options).ConfigureAwait(false);
            return response.Status;
        }

        internal async Task<HttpStatusCode> GetEmailVerificationStatusAsync(RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("HEAD", $"me/email_verified", options).ConfigureAwait(false);
            return response.Status;
        }

        internal async Task SendVerificationEmailRequestAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("POST", $"me/send_verification_email", options).ConfigureAwait(false);
        }

        internal async Task SendPasswordResetEmailAsync(string username, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PATCH", "users", new ActionRequest() { Action = "send_password_reset_email", Value = username }, options).ConfigureAwait(false);
        }

        internal async Task<Models.User> GetUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}", options).ConfigureAwait(false);
            return response.ReadAsJson<Models.User>(Config);
        }

        internal async Task<Models.CurrentUser> GetCurrentUserAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me", options).ConfigureAwait(false);
            return response.ReadAsJson<Models.CurrentUser>(Config);
        }

        internal async Task ModifyCurrentUserAsync(IEnumerable<GfycatOperation> operations, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PATCH", "me", new ModifyCurrentUserParameters() { Operations = operations }, options).ConfigureAwait(false);
        }

        internal async Task AddProviderAsync(AddProviderParameters parameter, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "me", parameter, options).ConfigureAwait(false);
        }

        internal async Task RemoveProviderAsync(string provider, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/{provider}", options).ConfigureAwait(false);
        }

        internal async Task<GeoWhitelistShared> GetGeoWhitelistAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/geo-whitelist", options).ConfigureAwait(false);
            return response.ReadAsJson<GeoWhitelistShared>(Config);
        }

        internal async Task<DomainWhitelistShared> GetDomainWhitelistAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/domain-whitelist", options).ConfigureAwait(false);
            return response.ReadAsJson<DomainWhitelistShared>(Config);
        }

        internal async Task ModifyGeoWhitelistAsync(GeoWhitelistShared modified, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", "me/geo-whitelist", modified, options).ConfigureAwait(false);
        }

        internal async Task ModifyDomainWhitelistAsync(DomainWhitelistShared modified, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", "me/domain-whitelist", modified, options).ConfigureAwait(false);
        }

        internal async Task DeleteGeoWhitelistAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", "me/geo-whitelist", options).ConfigureAwait(false);
        }

        internal async Task DeleteDomainWhitelistAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", "me/domain-whitelist", options).ConfigureAwait(false);
        }

        internal async Task<string> GetProfileUploadUrlAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("POST", "me/profile_image_url", options).ConfigureAwait(false);
            return Regex.Unescape(response.ReadAsString().Trim('"'));
        }

        internal async Task<string> GetProfileUrlAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/profile_image_url", options).ConfigureAwait(false);
            return Regex.Unescape(response.ReadAsString().Trim('"'));
        }

        internal async Task UploadProfileImageAsync(string url, Stream picStream, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.UseAccessToken = false;
            RestResponse response = await SendBinaryStreamAsync("PUT", url, picStream, options).ConfigureAwait(false);
        }

        internal async Task<ProfileImageUploadStatus> GetProfileImageUploadStatusAsync(string ticket, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/profile_image_url/status/{ticket}", options).ConfigureAwait(false);
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), response.ReadAsString().Trim('"'), true);
        }

        internal async Task<ClientAccountAuthResponse> CreateAccountAsync(AccountCreationRequest account, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "users", account, options);
            return response.ReadAsJson<ClientAccountAuthResponse>(Config);
        }

        internal async Task FollowUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("PUT", $"me/follows/{userId}", options).ConfigureAwait(false);
        }

        internal async Task UnfollowUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/follows/{userId}", options).ConfigureAwait(false);
        }

        internal async Task<HttpStatusCode> GetFollowingUserAsync(string userId, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("HEAD", $"me/follows/{userId}", options).ConfigureAwait(false);
            return response.Status;
        }

        internal async Task<FollowsResponse> GetFollowingsAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/follows", options).ConfigureAwait(false);
            return response.ReadAsJson<FollowsResponse>(Config);
        }

        internal async Task<FollowersResponse> GetFollowersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/followers", options).ConfigureAwait(false);
            return response.ReadAsJson<FollowersResponse>(Config);
        }

        #endregion

        #region User feeds

        internal async Task<Feed> GetUserGfyFeedAsync(string userId, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(( "cursor", cursor ));
            RestResponse response = await SendAsync("GET", $"users/{userId}/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        internal async Task<Models.Feed> GetCurrentUserGfyFeedAsync(string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(("cursor",cursor));
            RestResponse response = await SendAsync("GET", $"me/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        internal async Task<Models.Feed> GetFollowsGfyFeedAsync(string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(( "cursor", cursor ));
            RestResponse response = await SendAsync("GET", $"me/follows/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        #endregion

        #region User folders

        internal async Task<IEnumerable<Models.FolderInfo>> GetCurrentUserFoldersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/folders", options).ConfigureAwait(false);
            return response.ReadAsJson<IEnumerable<Models.FolderInfo>>(Config);
        }

        internal async Task<Models.Folder> GetFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/folders/{folderId}", options).ConfigureAwait(false);
            return response.ReadAsJson<Models.Folder>(Config);
        }
        
        internal async Task DeleteFolderAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/folders/{folderId}", options).ConfigureAwait(false);
        }

        internal async Task ModifyFolderTitleAsync(string folderId, string newTitle, RequestOptions options)
{
            RestResponse response = await SendJsonAsync("PUT", $"me/folders/{folderId}/name", new { value = newTitle }, options).ConfigureAwait(false);
        }

        internal async Task MoveFolderAsync(string folderId, string parentId, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/folders/{folderId}", new { parent_id = parentId }, options).ConfigureAwait(false);
        }

        internal async Task CreateFolderAsync(string folderId, string title, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", $"me/folders/{folderId}", new { folderName = title }, options).ConfigureAwait(false);
        }

        internal async Task MoveGfysAsync(string folderId, GfyFolderAction action, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PATCH", $"me/folders/{folderId}", action, options).ConfigureAwait(false);
        }

        #endregion

        #region Bookmarks

        internal async Task<IEnumerable<Models.FolderInfo>> GetCurrentUserBookmarkFoldersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/bookmark-folders", options).ConfigureAwait(false);
            return response.ReadAsJson<IEnumerable<Models.FolderInfo>>(Config);
        }

        internal async Task<Models.Folder> GetBookmarkFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/bookmark-folders/{folderId}", options).ConfigureAwait(false);
            return response.ReadAsJson<Models.Folder>(Config);
        }

        internal async Task DeleteBookmarkFolderAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/bookmark-folders/{folderId}", options).ConfigureAwait(false);
        }

        internal async Task ModifyBookmarkFolderTitleAsync(string folderId, string newTitle, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/bookmark-folders/{folderId}/name", new { value = newTitle }, options).ConfigureAwait(false);
        }

        internal async Task MoveBookmarkFolderAsync(string folderId, string parentId, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/bookmark-folders/{folderId}", new { parent_id = parentId }, options).ConfigureAwait(false);
        }

        internal async Task CreateBookmarkFolderAsync(string folderId, string title, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", $"me/bookmark-folders/{folderId}", new { folderName = title }, options).ConfigureAwait(false);
        }

        internal async Task MoveBookmarkedGfysAsync(string folderId, GfyFolderAction action, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PATCH", $"me/bookmark-folders/{folderId}", action, options).ConfigureAwait(false);
        }

        internal async Task<BookmarkedResult> GetBookmarkedStatusAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/bookmark-folders/{gfyId}", options);
            return response.ReadAsJson<BookmarkedResult>(Config);
        }

        internal async Task BookmarkGfyAsync(string gfyId, string folderId, RequestOptions options)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                await SendAsync("PUT", $"me/bookmark-folders/{gfyId}", options);
            else
                await SendAsync("PUT", $"me/bookmark-folders/{folderId}/contents/{gfyId}", options);
        }

        internal async Task UnbookmarkGfyAsync(string gfyId, string folderId, RequestOptions options)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                await SendAsync("DELETE", $"me/bookmark-folders/{gfyId}", options);
            else
                await SendAsync("DELETE", $"me/bookmark-folders/{folderId}/contents/{gfyId}", options);
        }

        #endregion

        #region Albums

        internal async Task<IEnumerable<Models.AlbumInfo>> GetAlbumsAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/album-folders", options);
            return response.ReadAsJson<IEnumerable<Models.AlbumInfo>>(Config);
        }

        internal async Task<IEnumerable<Models.AlbumInfo>> GetAlbumsForUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/album-folders", options);
            return response.ReadAsJson<IEnumerable<Models.AlbumInfo>>(Config);
        }

        internal async Task<Models.Album> GetAlbumContentsAsync(string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/albums/{albumId}", options);
            return response.ReadAsJson<Models.Album>(Config);
        }

        internal async Task<Models.Album> GetAlbumContentsAsync(string userId, string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/albums/{albumId}", options);
            return response.ReadAsJson<Models.Album>(Config);
        }

        internal async Task<Models.Album> GetAlbumContentsByLinkTextAsync(string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/album_links/{albumLinkText}", options);
            return response.ReadAsJson<Models.Album>(Config);
        }

        internal async Task<Models.Album> GetAlbumContentsByLinkTextAsync(string userId, string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/album_links/{albumLinkText}", options);
            return response.ReadAsJson<Models.Album>(Config);
        }

        internal async Task CreateAlbumAsync(string albumId, string name, RequestOptions options)
        {
            await SendJsonAsync("POST", $"me/albums/{albumId}", new { folderName = name }, options);
        }

        internal async Task MoveAlbumAsync(string albumId, string parentId, RequestOptions options)
        {
            await SendJsonAsync("PUT", $"me/albums/{albumId}", new { parentId }, options);
        }

        internal async Task AddGfysAsync(string albumId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new { gfy_ids = gfyIds }, options);
        }

        internal async Task MoveGfysAsync(string albumId, string parentId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new GfyFolderAction() { Action = "move_contents", GfycatIds = gfyIds, ParentId = parentId }, options);
        }

        internal async Task RemoveGfysAsync(string albumId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new ActionRequest() { Action = "remove_contents", Value = gfyIds }, options);
        }

        internal async Task CreateAlbumInFolderAsync(string folderId, string title, RequestOptions options)
        {
            await SendJsonAsync("POST", $"me/album-folders/{folderId}", new { folderName = title }, options);
        }

        internal async Task ModifyTitleAsync(string albumId, string newTitle, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/title", new { value = newTitle }, options);
        }

        internal async Task ModifyDescriptionAsync(string albumId, string newDescription, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/description", new { value = newDescription }, options);
        }

        internal async Task ModifyNsfwSettingAsync(string albumId, NsfwSetting setting, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/nsfw", new { value = setting }, options);
        }

        internal async Task ModifyPublishedSettingAsync(string albumId, bool published, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/published", new { value = published }, options);
        }

        internal async Task ModifyOrderAsync(string albumId, IEnumerable<string> gfyIdsInNewOrder, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/order", new { newOrder = gfyIdsInNewOrder }, options);
        }

        internal async Task DeleteAsync(string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/albums/{albumId}", options);
        }

        #endregion

        #region Getting gfycats

        public async Task<GfyResponse> GetGfyAsync(string gfyId, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("GET", $"gfycats/{gfyId}", options);
            if (response.Status == HttpStatusCode.NotFound)
                return null;

            return response.ReadAsJson<GfyResponse>(Config);
        }

        #endregion

        #region Creating gfycats

        internal async Task<UploadKey> CreateGfyFromFetchUrlAsync(string url, GfyCreationParameters parameters, RequestOptions options)
        {
            return await GetUploadKeyAsync(parameters, options);
        }

        internal async Task<Status> GetGfyStatusAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"gfycats/fetch/status/{gfyId}", options);
            Status tempStatus = response.ReadAsJson<Status>(Config);
            if (tempStatus.GfyName is null)
                tempStatus.GfyName = gfyId;
            return tempStatus;
        }

        internal async Task<UploadKey> GetUploadKeyAsync(GfyCreationParameters parameters, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "gfycats", parameters, options);
            return response.ReadAsJson<UploadKey>(Config);
        }

        internal async Task PostGfyStreamAsync(UploadKey key, Stream stream, RequestOptions options)
        {
            RestResponse response = await SendStreamAsync("POST", "https://filedrop.gfycat.com/", stream, key.Gfycat, options); // uploads as multipart
        }

        #endregion

        #region Updating gfycats

        internal async Task ModifyGfyTitleAsync(string gfyId, string newTitle, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/title", new { value = newTitle }, options);
        }

        internal async Task DeleteGfyTitleAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/title", options);
        }

        internal async Task ModifyGfyTagsAsync(string gfyId, IEnumerable<string> newTags, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/tags", new { value = newTags }, options);
        }

        internal async Task<IEnumerable<string>> GetGfyDomainWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/gfycats/{gfyId}/domain-whitelist", options);
            return response.ReadAsJson<DomainWhitelistShared>(Config).DomainWhitelist;
        }

        internal async Task ModifyGfyDomainWhitelistAsync(string gfyId, IEnumerable<string> newWhitelist, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/domain-whitelist", new DomainWhitelistShared() { DomainWhitelist = newWhitelist }, options);
        }

        internal async Task DeleteGfyDomainWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/domain-whitelist", options);
        }

        internal async Task<IEnumerable<string>> GetGfyGeoWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/gfycats/{gfyId}/geo-whitelist", options);
            return response.ReadAsJson<GeoWhitelistShared>(Config).GeoWhitelist;
        }
        
        internal async Task ModifyGfyGeoWhitelistAsync(string gfyId, IEnumerable<string> newWhitelist, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/geo-whitelist", new GeoWhitelistShared() { GeoWhitelist = newWhitelist }, options);
        }

        internal async Task DeleteGfyGeoWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/geo-whitelist", options);
        }

        internal async Task ModifyGfyDescriptionAsync(string gfyId, string newDescription, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/description", new { value = newDescription } ,options);
        }

        internal async Task DeleteGfyDescriptionAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/description", options);
        }

        internal async Task ModifyGfyPublishedSettingAsync(string gfyId, bool published, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/published", new { value = published }, options);
        }

        internal async Task ModifyGfyNsfwSettingAsync(string gfyId, NsfwSetting newSetting, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/nsfw", new { value = newSetting }, options);
        }

        internal async Task DeleteGfyAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}", options);
        }

        #endregion

        #region Sharing gfycats

        internal async Task ShareGfyAsync(string gfyId, TwitterShareRequest shareParams, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", $"gfycats/{gfyId}/share/twitter", shareParams, options);
        }

        #endregion

        #region Trending feeds

        internal async Task<TrendingFeed> GetTrendingFeedAsync(string tagName,string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("tagName", tagName), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/trending{queryString}", options);
            return response.ReadAsJson<TrendingFeed>(Config);
        }

        internal async Task<IEnumerable<string>> GetTrendingTagsAsync(string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending{queryString}", options);
            return response.ReadAsJson<IEnumerable<string>>(Config);
        }

        internal async Task<TrendingTagsFeed> GetTrendingTagsPopulatedAsync(string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending/populated{queryString}", options);
            return response.ReadAsJson<TrendingTagsFeed>(Config);
        }

        #endregion

        #region Search

        internal async Task<Feed> SearchSiteAsync(string searchText, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("search_text", searchText), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/search{queryString}", options);
            return response.ReadAsJson<Feed>(Config);
        }

        internal async Task<Feed> SearchCurrentUserAsync(string searchText, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("search_text", searchText), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"me/gfycats/search{queryString}", options);
            return response.ReadAsJson<Feed>(Config);
        }

        #endregion
        
        #region Developer API functions

        internal async Task<IEnumerable<ApplicationInfo>> GetDeveloperKeysAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/api-credentials", options);
            return response.ReadAsJson<IEnumerable<ApplicationInfo>>(Config);
        }

        #endregion
    }
}