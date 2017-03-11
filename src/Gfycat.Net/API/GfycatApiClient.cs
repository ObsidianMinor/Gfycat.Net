using Gfycat.API.Models;
using Gfycat.OAuth2;
using Gfycat.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gfycat.API
{
    internal class GfycatApiClient
    {
        internal IRestClient RestClient => Config.RestClient;
        internal GfycatClientConfig Config { get; }
        internal IAuthenticator Authentication { get; }

        internal GfycatApiClient(string id, string secret, GfycatClientConfig config)
        {
            Authentication = new DefaultAuthenticator(id, secret, this);
            Config = config;
        }

        #region Rest client abstractions
        
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

        internal async Task<RestResponse> SendStreamAsync(string method, string endpoint, string key, Stream stream, string fileName, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            Dictionary<string, object> content = new Dictionary<string, object>
            {
                { key, new MultipartFile(stream, fileName) }
            };
            return await SendInternalAsync(() => RestClient.SendAsync(method, endpoint, content, options.CancellationToken), options).ConfigureAwait(false);
        }

        private async Task<RestResponse> SendInternalAsync(Func<Task<RestResponse>> RestFunction, RequestOptions options)
        {
            RestResponse response = null;
            RestClient.SetAuthorization("Bearer", options.UseAccessToken ? Authentication.AccessToken : null);
            bool retry = true, first401 = true;
            while(retry)
            {
                Task<RestResponse> taskResponse = RestFunction();
                bool taskTimedout = !taskResponse.Wait(options.Timeout ?? -1);
                if (taskTimedout && options.RetryMode != RetryMode.RetryTimeouts) // the blocking wait call will complete the task, so in the following checks we can just access the result normally
                    throw new TimeoutException($"The request timed out");
                else if (taskResponse.Result.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502))
                    throw GfycatException.CreateFromResponse(taskResponse.Result);
                else if (taskResponse.Result.Status == HttpStatusCode.Unauthorized)
                    if (first401 && options.RetryMode.HasFlag(RetryMode.RetryFirst401))
                    {
                        await Authentication.RefreshTokenAsync();
                        first401 = false;
                    }
                    else
                        throw GfycatException.CreateFromResponse(taskResponse.Result);
                else
                {
                    response = taskResponse.Result;
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code != response.Status) ?? false))
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
            return response.ReadAsString();
        }

        internal async Task UploadProfileImageAsync(string url, Stream picStream, RequestOptions options)
        {
            throw new NotImplementedException();
        }

        internal async Task<ProfileImageUploadStatus> GetProfileImageUploadStatusAsync(string ticket, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/profile_image_url/status/{ticket}", options).ConfigureAwait(false);
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), response.ReadAsString(), true);
        }

        internal async Task<ClientAccountAuthResponse> CreateAccountAsync(object notImplemented, RequestOptions options)
        {
            throw new NotImplementedException();
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

        internal async Task<Models.Feed> GetUserGfyFeedAsync(string userId, int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(new Dictionary<string, object> { { "count", count }, { "cursor", cursor } });
            RestResponse response = await SendAsync("GET", $"users/{userId}/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        internal async Task<Models.Feed> GetCurrentUserGfyFeedAsync(int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(new Dictionary<string, object> { { "count", count }, { "cursor", cursor } });
            RestResponse response = await SendAsync("GET", $"me/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        internal async Task<Models.Feed> GetFollowsGfyFeedAsync(int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(new Dictionary<string, object> { { "count", count }, { "cursor", cursor } });
            RestResponse response = await SendAsync("GET", $"me/follows/gfycats{query}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        #endregion

        #region User folders

        internal async Task<FolderInfo> GetCurrentUserFoldersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/folders", options).ConfigureAwait(false);
            return response.ReadAsJson<FolderInfo>(Config);
        }

        internal async Task<Folder> GetFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/folders/{folderId}", options).ConfigureAwait(false);
            return response.ReadAsJson<Folder>(Config);
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

        internal async Task<FolderInfo> GetCurrentUserBookmarkFoldersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/bookmark-folders", options).ConfigureAwait(false);
            return response.ReadAsJson<FolderInfo>(Config);
        }

        internal async Task<Folder> GetBookmarkFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/bookmark-folders/{folderId}", options).ConfigureAwait(false);
            return response.ReadAsJson<Folder>(Config);
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

        internal async Task<IEnumerable<AlbumInfo>> GetAlbumsAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/album-folders", options);
            return response.ReadAsJson<IEnumerable<AlbumInfo>>(Config);
        }

        internal async Task<IEnumerable<AlbumInfo>> GetAlbumsForUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/album-folders", options);
            return response.ReadAsJson<IEnumerable<AlbumInfo>>(Config);
        }

        internal async Task<Album> GetAlbumContentsAsync(string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/albums/{albumId}", options);
            return response.ReadAsJson<Album>(Config);
        }

        internal async Task<Album> GetAlbumContentsAsync(string userId, string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/albums/{albumId}", options);
            return response.ReadAsJson<Album>(Config);
        }

        internal async Task<Album> GetAlbumContentsByLinkTextAsync(string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/album_links/{albumLinkText}", options);
            return response.ReadAsJson<Album>(Config);
        }

        internal async Task<Album> GetAlbumContentsByLinkTextAsync(string userId, string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/album_links/{albumLinkText}", options);
            return response.ReadAsJson<Album>(Config);
        }

        internal async Task CreateAlbumAsync(string albumId, RequestOptions options)
        {
            await SendAsync("POST", $"me/albums/{albumId}", options);
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

        internal async Task CreateAlbumInFolderAsync(string folderId, string title, string description, IEnumerable<string> gfyIds, RequestOptions options)
        {

        }

        internal async Task ModifyTitleAsync(string albumId, string newTitle, RequestOptions options)
        {

        }

        internal async Task ModifyDescriptionAsync(string albumId, string newDescription, RequestOptions options)
        {

        }

        internal async Task ModifyNsfwSettingAsync(string albumId, NsfwSetting setting, RequestOptions options)
        {

        }

        internal async Task ModifyPublishedSettingAsync(string albumId, bool published, RequestOptions options)
        {

        }

        internal async Task ModifyOrderAsync(string albumId, IEnumerable<string> gfyIdsInNewOrder, RequestOptions options)
        {

        }

        #endregion

        #region Getting gfycats

        public async Task<GfyResponse> GetGfyAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "gfycats/{gfyId}", options);
            return response.ReadAsJson<GfyResponse>(Config);
        }

        #endregion

        #region Creating gfycats

        internal async Task<Models.UploadKey> CreateGfyFromFetchUrlAsync(string url, GfyCreationParameters parameters, RequestOptions options)
        {
            parameters.FetchUrl = url;
            return await GetUploadKeyAsync(parameters, options);
        }

        internal async Task<Models.Status> GetGfyStatusAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"gfycats/fetch/status/{gfyId}", options);
            return response.ReadAsJson<Models.Status>(Config);
        }

        internal async Task<Models.UploadKey> GetUploadKeyAsync(GfyCreationParameters parameters, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "gfycats", parameters, options);
            return response.ReadAsJson<Models.UploadKey>(Config);
        }

        #endregion

        #region Updating gfycats

        

        #endregion

        #region Sharing gfycats

        internal async Task ShareGfyAsync(string gfyId, TwitterShareRequest shareParams, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", $"gfycats/{gfyId}/share/twitter", shareParams, options);
        }

        #endregion

        #region Trending feeds

        internal async Task<TrendingFeed> GetTrendingFeedAsync(string tagName, int count, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("tagName", tagName), ("count", count), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/trending{queryString}", options);
            return response.ReadAsJson<TrendingFeed>(Config);
        }

        internal async Task<IEnumerable<string>> GetTrendingTagsAsync(int tagCount, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("tagCount", tagCount), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending{queryString}", options);
            return response.ReadAsJson<IEnumerable<string>>(Config);
        }

        internal async Task<Feed> GetTrendingTagsPopulatedAsync(int tagCount, int gfyCount, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("tagCount", tagCount), ("gfyCount", gfyCount), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending/populated{queryString}", options);
            return response.ReadAsJson<Models.Feed>(Config);
        }

        #endregion

        #region Search

        internal async Task<Feed> SearchSiteAsync(string searchText, int count, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("search_text", searchText), ("count", count), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/search{queryString}", options);
            return response.ReadAsJson<Feed>(Config);
        }

        internal async Task<Feed> SearchCurrentUserAsync(string searchText, int count, string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("search_text", searchText), ("count", count), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"me/gfycats/search{queryString}", options);
            return response.ReadAsJson<Feed>(Config);
        }

        #endregion

        #region Developer API functions



        #endregion
    }
}