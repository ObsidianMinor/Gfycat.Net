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
    public class CurrentUser : ConnectedEntity, IUser
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

        public async Task<bool> GetEmailVerifiedAsync()
        {
            HttpStatusCode code = await Web.SendRequestForStatusAsync("GET", "me/email-verified", throwIf401: true);
            return code != HttpStatusCode.NotFound;
        }

        public async Task SendVerificationEmailAsync()
        {
            await Web.SendRequestAsync("POST", "me/send_verification_email");
        }

        public async Task<string> UploadProfilePictureAsync(Stream profilePic, CancellationToken? cancel = null)
        {
            string ticketResponse = await Web.SendRequestForStringAsync("POST", "me/profile_image_url");
            string ticket = ticketResponse.Split('/').Last();
            await Web.SendStreamAsync("PUT", $"https://imageupload.gfycat.com/{ticket}", profilePic, null, throwIf401:true, cancelToken:cancel);
            return ticket;
        }

        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(string ticket)
        {
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), (await Web.SendRequestForStringAsync("GET", $"me/profile_image_url/status/{ticket}")), true);
        }

        public async Task ModifyCurrentUserAsync(params GfycatOperation[] operations)
        {
            await Web.SendJsonAsync("PATCH", "me", new { operations });
        }

        public Task FollowUserAsync(string userId)
        {
            return Web.SendRequestAsync("PUT", $"me/follows/{userId}");
        }

        public Task UnfollowUserAsync(string userId)
        {
            return Web.SendRequestAsync("DELETE", $"me/follows/{userId}");
        }

        /// <summary>
        /// Checks if this user is following the provided user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if the current user is following the provided user, otherwise false</returns>
        public async Task<bool> GetFollowingUserAsync(string userId)
        {
            return (await Web.SendRequestForStatusAsync("HEAD", $"me/follows/{userId}")) == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns an enumerable of users the current user is following
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetFollowingUsersAsync()
        {
            return Web.SendRequestAsync<IEnumerable<string>>("GET", "me/follows");
        }

        /// <summary>
        /// Returns an enumerable of users following the current user
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetUsersFollowingAsync()
        {
            return Web.SendRequestAsync<IEnumerable<string>>("GET", "me/following");
        }

        #endregion

        #region User feeds

        public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/gfycats{queryString}");
        }

        public Task<GfycatFeed> GetTimelineFeedAsync(int? count = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/follows/gfycats{queryString}");
        }

        #endregion

        #region User Folders

        /// <summary>
        /// Retrieves a list of folder information for the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GfycatFolderInfo>> GetFoldersAsync()
        {
            return (await Web.SendRequestAsync<GfycatFolderInfo>("GET", "me/folders")).Subfolders;
        }

        public Task CreateFolderAsync(string folderName, string parent = null)
        {
            return Web.SendJsonAsync("POST", $"me/folders{(string.IsNullOrWhiteSpace(parent) ? "" : $"/{parent}")}", new { folderName });
        }

        #endregion

        #region Bookmarks

        public Task<IEnumerable<GfycatBookmarkFolderInfo>> GetBookmarkFoldersAsync()
        {
            return Web.SendRequestAsync<IEnumerable<GfycatBookmarkFolderInfo>>("GET", "me/bookmark-folders");
        }

        #endregion

        #region Albums

        public async Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync()
        {
            IEnumerable<GfycatAlbumInfo> albums = (await Web.SendRequestAsync<GfycatAlbumResponse>("GET", $"me/album-folders")).Albums;
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

        public Task<GfycatFeed> SearchAsync(string searchText, int? count = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>
            {
                { "search_text", searchText },
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/gfycats/search{queryString}");
        }
    }
}
