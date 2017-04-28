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
            return await SendInternalAsync(() => Config.RestClient.SendAsync(method, endpoint, options.CancellationToken), options).ConfigureAwait(false);
        }

        internal async Task<RestResponse> SendJsonAsync(string method, string endpoint, object jsonObject, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            string jsonString = JsonConvert.SerializeObject(jsonObject, Config.SerializerSettings);
            return await SendInternalAsync(() => Config.RestClient.SendAsync(method, endpoint, jsonString, options.CancellationToken), options).ConfigureAwait(false);
        }

        internal async Task<RestResponse> SendStreamAsync(string method, string endpoint, Stream stream, string fileName, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            Dictionary<string, object> content = new Dictionary<string, object>
            {
                { "key", fileName },
                { "file", stream }
            };
            return await SendInternalAsync(() => Config.RestClient.SendAsync(method, endpoint, content, options.CancellationToken), options).ConfigureAwait(false);
        }

        private async Task<RestResponse> SendBinaryStreamAsync(string method, string endpoint, Stream stream, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            return await SendInternalAsync(() => Config.RestClient.SendAsync(method, endpoint, stream, options.CancellationToken), options).ConfigureAwait(false);
        }

        private async Task<RestResponse> SendInternalAsync(Func<Task<RestResponse>> restFunction, RequestOptions options)
        {
            RestResponse response = null;
            Config.RestClient.SetHeader("Authorization", options.UseAccessToken ? $"Bearer {Client.AccessToken}" : null);
            bool retry = true, first401 = true;
            while (retry)
            {
                try // create the task and run it
                {
                    Task<RestResponse> task = restFunction();
                    response = await task.TimeoutAfter(options.Timeout).ConfigureAwait(false);
                }
                catch (TimeoutException) when (options.RetryMode.HasFlag(RetryMode.RetryTimeouts)) // catch the timeout if specified, then try again by continuing
                {
                    continue;
                }
                if (response.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502)) // if there was a bad gateway and we don't retry them, throw
                    throw await GfycatException.CreateFromResponseAsync(response).ConfigureAwait(false);
                else if (response.Status == HttpStatusCode.Unauthorized)
                    if (options.UseAccessToken && (first401 && options.RetryMode.HasFlag(RetryMode.RetryFirst401))) // make sure we don't get in a refresh loop due to not having an access token when using an invalid client ID
                    {
                        await Client.RefreshTokenAsync().ConfigureAwait(false);
                        Config.RestClient.SetHeader("Authorization", options.UseAccessToken ? $"Bearer {Client.AccessToken}" : null);
                        first401 = false;
                        continue;
                    }
                    else
                        throw await GfycatException.CreateFromResponseAsync(response).ConfigureAwait(false);
                else
                {
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code == response.Status) ?? false)) // make sure the status, if an error, isn't ignored
                        throw await GfycatException.CreateFromResponseAsync(response).ConfigureAwait(false);
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
            return await response.ReadAsJsonAsync<Models.User>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.CurrentUser> GetCurrentUserAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.CurrentUser>(Config).ConfigureAwait(false);
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
            return await response.ReadAsJsonAsync<GeoWhitelistShared>(Config).ConfigureAwait(false);
        }

        internal async Task<DomainWhitelistShared> GetDomainWhitelistAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/domain-whitelist", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<DomainWhitelistShared>(Config).ConfigureAwait(false);
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
            return Regex.Unescape((await response.ReadAsStringAsync().ConfigureAwait(false)).Trim('"'));
        }

        internal async Task<string> GetProfileUrlAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/profile_image_url", options).ConfigureAwait(false);
            return Regex.Unescape((await response.ReadAsStringAsync().ConfigureAwait(false)).Trim('"'));
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
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), (await response.ReadAsStringAsync().ConfigureAwait(false)).Trim('"'), true);
        }

        internal async Task<ClientAccountAuthResponse> CreateAccountAsync(AccountCreationRequest account, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "users", account, options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<ClientAccountAuthResponse>(Config).ConfigureAwait(false);
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
            return await response.ReadAsJsonAsync<FollowsResponse>(Config).ConfigureAwait(false);
        }

        internal async Task<FollowersResponse> GetFollowersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/followers", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<FollowersResponse>(Config).ConfigureAwait(false);
        }

        #endregion

        #region User feeds

        internal async Task<Feed> GetUserGfyFeedAsync(string userId, int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(("count", count), ( "cursor", cursor ));
            RestResponse response = await SendAsync("GET", $"users/{userId}/gfycats{query}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Feed>(Config).ConfigureAwait(false);
        }

        internal async Task<Feed> GetCurrentUserGfyFeedAsync(int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(("count", count), ("cursor",cursor));
            RestResponse response = await SendAsync("GET", $"me/gfycats{query}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Feed>(Config).ConfigureAwait(false);
        }

        internal async Task<Feed> GetFollowsGfyFeedAsync(int count, string cursor, RequestOptions options)
        {
            string query = Utils.CreateQueryString(("count", count), ("cursor", cursor ));
            RestResponse response = await SendAsync("GET", $"me/follows/gfycats{query}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Feed>(Config).ConfigureAwait(false);
        }

        #endregion

        #region User folders

        internal async Task<IEnumerable<Models.FolderInfo>> GetCurrentUserFoldersAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/folders", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<IEnumerable<Models.FolderInfo>>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.Folder> GetFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/folders/{folderId}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Folder>(Config).ConfigureAwait(false);
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
            return await response.ReadAsJsonAsync<IEnumerable<Models.FolderInfo>>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.Folder> GetBookmarkFolderContentsAsync(string folderId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/bookmark-folders/{folderId}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Folder>(Config).ConfigureAwait(false);
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
            RestResponse response = await SendAsync("GET", $"me/bookmark-folders/{gfyId}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<BookmarkedResult>(Config).ConfigureAwait(false);
        }

        internal async Task BookmarkGfyAsync(string gfyId, string folderId, RequestOptions options)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                await SendAsync("PUT", $"me/bookmark-folders/{gfyId}", options).ConfigureAwait(false);
            else
                await SendAsync("PUT", $"me/bookmark-folders/{folderId}/contents/{gfyId}", options).ConfigureAwait(false);
        }

        internal async Task UnbookmarkGfyAsync(string gfyId, string folderId, RequestOptions options)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                await SendAsync("DELETE", $"me/bookmark-folders/{gfyId}", options).ConfigureAwait(false);
            else
                await SendAsync("DELETE", $"me/bookmark-folders/{folderId}/contents/{gfyId}", options).ConfigureAwait(false);
        }

        #endregion

        #region Albums

        internal async Task<IEnumerable<Models.AlbumInfo>> GetAlbumsAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/album-folders", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<IEnumerable<Models.AlbumInfo>>(Config).ConfigureAwait(false);
        }
        
        internal async Task<IEnumerable<Models.AlbumInfo>> GetAlbumsForUserAsync(string userId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/albums", options).ConfigureAwait(false);
            return (await response.ReadAsJsonAsync<UserAlbumsResponse>(Config).ConfigureAwait(false)).Items;
        }

        internal async Task<Models.Album> GetAlbumContentsAsync(string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/albums/{albumId}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Album>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.Album> GetAlbumContentsAsync(string userId, string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/albums/{albumId}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Album>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.Album> GetAlbumContentsByLinkTextAsync(string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/album_links/{albumLinkText}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Album>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.Album> GetAlbumContentsByLinkTextAsync(string userId, string albumLinkText, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"users/{userId}/album_links/{albumLinkText}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.Album>(Config).ConfigureAwait(false);
        }

        internal async Task CreateAlbumAsync(string albumId, string name, RequestOptions options)
        {
            await SendJsonAsync("POST", $"me/albums/{albumId}", new { folderName = name }, options).ConfigureAwait(false);
        }

        internal async Task MoveAlbumAsync(string albumId, string parentId, RequestOptions options)
        {
            await SendJsonAsync("PUT", $"me/albums/{albumId}", new { parentId }, options).ConfigureAwait(false);
        }

        internal async Task AddGfysAsync(string albumId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new { gfy_ids = gfyIds }, options).ConfigureAwait(false);
        }

        internal async Task MoveGfysAsync(string albumId, string parentId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new GfyFolderAction() { Action = "move_contents", GfycatIds = gfyIds, ParentId = parentId }, options).ConfigureAwait(false);
        }

        internal async Task RemoveGfysAsync(string albumId, IEnumerable<string> gfyIds, RequestOptions options)
        {
            await SendJsonAsync("PATCH", $"me/albums/{albumId}", new ActionRequest() { Action = "remove_contents", Value = gfyIds }, options).ConfigureAwait(false);
        }

        internal async Task CreateAlbumInFolderAsync(string folderId, string title, RequestOptions options)
        {
            await SendJsonAsync("POST", $"me/album-folders/{folderId}", new { folderName = title }, options).ConfigureAwait(false);
        }

        internal async Task ModifyTitleAsync(string albumId, string newTitle, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/title", new { value = newTitle }, options).ConfigureAwait(false);
        }

        internal async Task ModifyDescriptionAsync(string albumId, string newDescription, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/description", new { value = newDescription }, options).ConfigureAwait(false);
        }

        internal async Task ModifyNsfwSettingAsync(string albumId, NsfwSetting setting, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/nsfw", new { value = setting }, options).ConfigureAwait(false);
        }

        internal async Task ModifyPublishedSettingAsync(string albumId, bool published, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/published", new { value = published }, options).ConfigureAwait(false);
        }

        internal async Task ModifyOrderAsync(string albumId, IEnumerable<string> gfyIdsInNewOrder, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/albums/{albumId}/order", new { newOrder = gfyIdsInNewOrder }, options).ConfigureAwait(false);
        }

        internal async Task DeleteAsync(string albumId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/albums/{albumId}", options).ConfigureAwait(false);
        }

        #endregion

        #region Getting gfycats

        public async Task<GfyResponse> GetGfyAsync(string gfyId, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("GET", $"gfycats/{gfyId}", options).ConfigureAwait(false);
            if (response.Status == HttpStatusCode.NotFound)
                return null;

            return await response.ReadAsJsonAsync<GfyResponse>(Config).ConfigureAwait(false);
        }

        public async Task<FullGfyResponse> GetFullGfyAsync(string gfyId, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.IgnoreCodes = Utils.Ignore404;
            RestResponse response = await SendAsync("GET", $"me/gfycats/{gfyId}/full", options).ConfigureAwait(false);
            if (response.Status == HttpStatusCode.NotFound)
                return null;

            return await response.ReadAsJsonAsync<FullGfyResponse>(Config).ConfigureAwait(false);
        }

        #endregion

        #region Creating gfycats

        internal async Task<UploadKey> CreateGfyFromFetchUrlAsync(string url, GfyCreationParameters parameters, RequestOptions options)
        {
            parameters.FetchUrl = url;
            return await GetUploadKeyAsync(parameters.CreateModel(), options).ConfigureAwait(false);
        }

        internal async Task<Status> GetGfyStatusAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"gfycats/fetch/status/{gfyId}", options).ConfigureAwait(false);
            Status tempStatus = await response.ReadAsJsonAsync<Status>(Config).ConfigureAwait(false);
            if (tempStatus.GfyName is null)
                tempStatus.GfyName = gfyId;
            return tempStatus;
        }

        internal async Task<UploadKey> GetUploadKeyAsync(GfyParameters parameters, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", "gfycats", parameters, options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<UploadKey>(Config).ConfigureAwait(false);
        }

        internal async Task PostGfyStreamAsync(UploadKey key, Stream stream, RequestOptions options)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            options.UseAccessToken = false;
            RestResponse response = await SendStreamAsync("POST", "https://filedrop.gfycat.com/", stream, key.Gfycat, options).ConfigureAwait(false); // uploads as multipart
        }

        #endregion

        #region Updating gfycats

        internal async Task ModifyGfyTitleAsync(string gfyId, string newTitle, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/title", new { value = newTitle }, options).ConfigureAwait(false);
        }

        internal async Task DeleteGfyTitleAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/title", options).ConfigureAwait(false);
        }

        internal async Task ModifyGfyTagsAsync(string gfyId, IEnumerable<string> newTags, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/tags", new { value = newTags }, options).ConfigureAwait(false);
        }

        internal async Task<IEnumerable<string>> GetGfyDomainWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/gfycats/{gfyId}/domain-whitelist", options).ConfigureAwait(false);
            return (await response.ReadAsJsonAsync<DomainWhitelistShared>(Config).ConfigureAwait(false)).DomainWhitelist;
        }

        internal async Task ModifyGfyDomainWhitelistAsync(string gfyId, IEnumerable<string> newWhitelist, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/domain-whitelist", new DomainWhitelistShared() { DomainWhitelist = newWhitelist }, options).ConfigureAwait(false);
        }

        internal async Task DeleteGfyDomainWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/domain-whitelist", options).ConfigureAwait(false);
        }

        internal async Task<IEnumerable<string>> GetGfyGeoWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", $"me/gfycats/{gfyId}/geo-whitelist", options).ConfigureAwait(false);
            return (await response.ReadAsJsonAsync<GeoWhitelistShared>(Config).ConfigureAwait(false)).GeoWhitelist;
        }
        
        internal async Task ModifyGfyGeoWhitelistAsync(string gfyId, IEnumerable<string> newWhitelist, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/geo-whitelist", new GeoWhitelistShared() { GeoWhitelist = newWhitelist }, options).ConfigureAwait(false);
        }

        internal async Task DeleteGfyGeoWhitelistAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/geo-whitelist", options).ConfigureAwait(false);
        }

        internal async Task ModifyGfyDescriptionAsync(string gfyId, string newDescription, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/description", new { value = newDescription } ,options).ConfigureAwait(false);
        }

        internal async Task DeleteGfyDescriptionAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}/description", options).ConfigureAwait(false);
        }

        internal async Task ModifyGfyPublishedSettingAsync(string gfyId, bool published, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/published", new { value = published }, options).ConfigureAwait(false);
        }

        internal async Task ModifyGfyNsfwSettingAsync(string gfyId, NsfwSetting newSetting, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/nsfw", new { value = newSetting }, options).ConfigureAwait(false);
        }
        
        internal async Task LikeGfyAsync(string gfyid, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyid}/like", new { value = 1 }, options).ConfigureAwait(false);
        }

        internal async Task RemoveLikeGfyAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyId}/like", new { value = 0 }, options).ConfigureAwait(false);
        }

        internal async Task DislikeGfyAsync(string gfyid, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyid}/dislike", new { value = 1 }, options).ConfigureAwait(false);
        }

        internal async Task RemoveDislikeGfyAsync(string gfyid, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("PUT", $"me/gfycats/{gfyid}/dislike", new { value = 0 }, options).ConfigureAwait(false);
        }

        internal async Task DeleteGfyAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("DELETE", $"me/gfycats/{gfyId}", options).ConfigureAwait(false);
        }

        internal async Task ReportContentAsync(string gfyId, RequestOptions options)
        {
            RestResponse response = await SendAsync("OPTIONS", $"gfycats/{gfyId}/report-content", options).ConfigureAwait(false);
        }

        #endregion

        #region Sharing gfycats

        internal async Task ShareGfyAsync(string gfyId, TwitterShareRequest shareParams, RequestOptions options)
        {
            RestResponse response = await SendJsonAsync("POST", $"gfycats/{gfyId}/share/twitter", shareParams, options).ConfigureAwait(false);
        }

        #endregion

        #region Reaction gifs

        internal async Task<TrendingTagsFeed> GetReactionGifsPopulatedAsync(ReactionLanguage lang, int gfyCount, string cursor, RequestOptions options)
        {
            if (gfyCount <= GfycatClient.UseDefaultFeedCount)
                gfyCount = Config.DefaultFeedItemCount;

            string query = Utils.CreateQueryString(("locale", Utils._reactionLangToString[lang]), ("gfyCount", gfyCount), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", "reactions/populated" + query, options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<TrendingTagsFeed>(Config).ConfigureAwait(false);
        }

        #endregion

        #region Trending feeds

        internal async Task<TrendingFeed> GetTrendingFeedAsync(string tagName, int count, string cursor, RequestOptions options)
        {
            if (count <= GfycatClient.UseDefaultFeedCount)
                count = Config.DefaultFeedItemCount;

            string queryString = Utils.CreateQueryString(("count", count), ("tagName", tagName), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/trending{queryString}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<TrendingFeed>(Config).ConfigureAwait(false);
        }

        internal async Task<IEnumerable<string>> GetTrendingTagsAsync(string cursor, RequestOptions options)
        {
            string queryString = Utils.CreateQueryString(("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending{queryString}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<IEnumerable<string>>(Config).ConfigureAwait(false);
        }

        internal async Task<TrendingTagsFeed> GetTrendingTagsPopulatedAsync(string cursor, int tagCount, int gfyCount, RequestOptions options)
        {
            if (gfyCount <= GfycatClient.UseDefaultFeedCount)
                gfyCount = Config.DefaultFeedItemCount;

            if (tagCount <= GfycatClient.UseDefaultFeedCount)
                tagCount = Config.DefaultFeedItemCount;

            string queryString = Utils.CreateQueryString(("tagCount", tagCount), ("gfyCount", gfyCount), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"tags/trending/populated{queryString}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<TrendingTagsFeed>(Config).ConfigureAwait(false);
        }

        #endregion

        #region Search

        internal async Task<Models.SearchFeed> SearchSiteAsync(string searchText, int count, string cursor, RequestOptions options)
        {
            if (count <= GfycatClient.UseDefaultFeedCount)
                count = Config.DefaultFeedItemCount;

            string queryString = Utils.CreateQueryString(("search_text", searchText), ("count", count), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"gfycats/search{queryString}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.SearchFeed>(Config).ConfigureAwait(false);
        }

        internal async Task<Models.SearchFeed> SearchCurrentUserAsync(string searchText, int count, string cursor, RequestOptions options)
        {
            if (count <= GfycatClient.UseDefaultFeedCount)
                count = Config.DefaultFeedItemCount;

            string queryString = Utils.CreateQueryString(("search_text", searchText), ("count", count), ("cursor", cursor));
            RestResponse response = await SendAsync("GET", $"me/gfycats/search{queryString}", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<Models.SearchFeed>(Config).ConfigureAwait(false);
        }

        #endregion
        
        #region Developer API functions

        internal async Task<IEnumerable<ApplicationInfo>> GetDeveloperKeysAsync(RequestOptions options)
        {
            RestResponse response = await SendAsync("GET", "me/api-credentials", options).ConfigureAwait(false);
            return await response.ReadAsJsonAsync<IEnumerable<ApplicationInfo>>(Config).ConfigureAwait(false);
        }

        #endregion
    }
}