using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class CurrentUser : Entity, IUser
    {
        [JsonProperty("userid")]
        public string Id { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("profileUrl")]
        public string ProfileUrl { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("views")]
        public int Views { get; private set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; private set; }
        [JsonProperty("url")]
        public string Url { get; private set; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; private set; }
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; private set; }
        [JsonProperty("verified")]
        public bool Verified { get; private set; }
        [JsonProperty("followers")]
        public int Followers { get; private set; }
        [JsonProperty("following")]
        public int Following { get; private set; }
        [JsonProperty("iframeProfileImageVisible")]
        public bool IframeProfileImageVisible { get; private set; }
        [JsonProperty("geoWhitelist")]
        public string GeoWhitelist { get; private set; }
        [JsonProperty("domainWhitelist")]
        public string DomainWhitelist { get; private set; }
        [JsonProperty("email")]
        public string Email { get; private set; }

        #region Users

        public async Task<bool> GetEmailVerifiedAsync(RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Client.Config);
            options.IgnoreCodes = Utils.Ignore404;
            HttpStatusCode code = (await Client.SendAsync("GET", "me/email-verified", options)).Status;
            return code != HttpStatusCode.NotFound;
        }

        public async Task SendVerificationEmailAsync(RequestOptions options = null)
        {
            await Client.SendAsync("POST", "me/send_verification_email", options);
        }

        public async Task<string> UploadProfilePictureAsync(Stream profilePic, RequestOptions options = null)
        {
            string ticketResponse = (await Client.SendAsync("POST", "me/profile_image_url", options)).ReadAsString();
            string ticket = ticketResponse.Split('/').Last();
            await Client.SendStreamAsync("PUT", $"https://imageupload.gfycat.com/{ticket}","profilePic", profilePic, null, options);
            return ticket;
        }

        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(string ticket, RequestOptions options = null)
        {
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), (await Client.SendAsync("GET", $"me/profile_image_url/status/{ticket}", options)).ReadAsString(), true);
        }

        public async Task ModifyCurrentUserAsync(GfycatOperation[] operations, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PATCH", "me", new { operations });
        }

        public Task FollowUserAsync(string userId, RequestOptions options = null)
        {
            return Client.SendAsync("PUT", $"me/follows/{userId}", options);
        }

        public Task UnfollowUserAsync(string userId, RequestOptions options = null)
        {
            return Client.SendAsync("DELETE", $"me/follows/{userId}", options);
        }

        /// <summary>
        /// Checks if this user is following the provided user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if the current user is following the provided user, otherwise false</returns>
        public async Task<bool> GetFollowingUserAsync(string userId, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Client.Config);
            options.IgnoreCodes = Utils.Ignore404;
            return (await Client.SendAsync("HEAD", $"me/follows/{userId}", options)).Status == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns an enumerable of users the current user is following
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetFollowingUsersAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", "me/follows", options);
        }

        /// <summary>
        /// Returns an enumerable of users following the current user
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetUsersFollowingAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", "me/following", options);
        }

        #endregion

        #region User feeds

        public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Client.SendAsync<GfycatFeed>("GET", $"me/gfycats{queryString}", options);
        }

        public Task<GfycatFeed> GetTimelineFeedAsync(int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Client.SendAsync<GfycatFeed>("GET", $"me/follows/gfycats{queryString}", options);
        }

        #endregion

        #region User Folders

        /// <summary>
        /// Retrieves a list of folder information for the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GfycatFolderInfo>> GetFoldersAsync(RequestOptions options = null)
        {
            return (await Client.SendAsync<GfycatFolderInfo>("GET", "me/folders", options)).Subfolders;
        }

        public Task CreateFolderAsync(string folderName, string parent = null, RequestOptions options = null)
        {
            return Client.SendJsonAsync("POST", $"me/folders{(string.IsNullOrWhiteSpace(parent) ? "" : $"/{parent}")}", new { folderName }, options);
        }

        #endregion

        #region Bookmarks

        public Task<IEnumerable<GfycatBookmarkFolderInfo>> GetBookmarkFoldersAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<GfycatBookmarkFolderInfo>>("GET", "me/bookmark-folders", options);
        }

        #endregion

        #region Albums

        public async Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync(RequestOptions options = null)
        {
            IEnumerable<GfycatAlbumInfo> albums = (await Client.SendAsync<GfycatAlbumResponse>("GET", $"me/album-folders", options)).Albums;
            RecursiveSetOwners(albums);
            return albums;
        }

        private void RecursiveSetOwners(IEnumerable<GfycatAlbumInfo> albums)
        {
            foreach (GfycatAlbumInfo album in albums)
            {
                album.Owner = this;
                RecursiveSetOwners(album.Subalbums);
            }
        }
        
        #endregion

        public Task<GfycatFeed> SearchAsync(string searchText, int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>
            {
                { "search_text", searchText },
                { "count", count },
                { "cursor", cursor }
            });
            return Client.SendAsync<GfycatFeed>("GET", $"me/gfycats/search{queryString}", options);
        }
    }
}
