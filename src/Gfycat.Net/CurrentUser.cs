using Gfycat.API.Responses;
using Gfycat.Converters;
using Gfycat.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents the current login user on Gfycat
    /// </summary>
    public class CurrentUser : Entity, IUser
    {
        /// <summary>
        /// A unique identifier for the user
        /// </summary>
        [JsonProperty("userid")]
        public string Id { get; private set; }
        /// <summary>
        /// The user’s username on Gfycat
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }
        /// <summary>
        /// The user’s profile description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; private set; }
        /// <summary>
        /// The user’s profile link
        /// </summary>
        [JsonProperty("profileUrl")]
        public string ProfileUrl { get; private set; }
        /// <summary>
        /// The user’s name on Gfycat
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
        /// <summary>
        /// The number of user’s gfy views on Gfycat
        /// </summary>
        [JsonProperty("views")]
        public int Views { get; private set; }
        /// <summary>
        /// The user’s email verification status
        /// </summary>
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; private set; }
        /// <summary>
        /// The URL to the user’s profile on Gfycat
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; private set; }
        /// <summary>
        /// The date the user created their account
        /// </summary>
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; private set; }
        /// <summary>
        /// The URL to the user’s avatar on Gfycat
        /// </summary>
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; private set; }
        /// <summary>
        /// The account’s verified status
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; private set; }
        /// <summary>
        /// The number of user’s followers
        /// </summary>
        [JsonProperty("followers")]
        public int Followers { get; private set; }
        /// <summary>
        /// The number of users this user follows
        /// </summary>
        [JsonProperty("following")]
        public int Following { get; private set; }
        /// <summary>
        /// The user’s profile image visibility on the iframe
        /// </summary>
        [JsonProperty("iframeProfileImageVisible")]
        public bool IframeProfileImageVisible { get; private set; }
        /// <summary>
        /// The user’s geo whitelist on Gfycat
        /// </summary>
        [JsonProperty("geoWhitelist")]
        public string GeoWhitelist { get; private set; }
        /// <summary>
        /// The user’s domain whitelist on Gfycat
        /// </summary>
        [JsonProperty("domainWhitelist")]
        public string DomainWhitelist { get; private set; }
        /// <summary>
        /// The email address of the specified user
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; private set; }
        /// <summary>
        /// The user’s associated provider details (has the user linked their facebook or twitter account and selected details from the provider)
        /// </summary>
        [JsonProperty("associatedProvders")]
        public string AssociatedProviders { get; private set; }
        /// <summary>
        /// The user’s upload notices settings, whether the user wants to get notified of uploads or not
        /// </summary>
        [JsonProperty("uploadNotices")]
        public bool UploadNotices { get; private set; }

        public async Task UpdateAsync(RequestOptions options = null)
        {
            JsonConvert.PopulateObject((await Client.SendAsync("GET", "me", options)).ReadAsString(), this);
        }

        #region Users

        /// <summary>
        /// Gets whether the current user's email is verified
        /// </summary>
        /// <param name="options"></param>
        /// <returns>True if the current user's email is verified, otherwise false</returns>
        public async Task<bool> GetEmailVerifiedAsync(RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Client.Config);
            options.IgnoreCodes = Utils.Ignore404;
            HttpStatusCode code = (await Client.SendAsync("GET", "me/email-verified", options)).Status;
            return code != HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Sends a verification email to the current user's email address
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendVerificationEmailAsync(RequestOptions options = null)
        {
            await Client.SendAsync("POST", "me/send_verification_email", options);
        }

        /// <summary>
        /// Modifies the current user's profile pic
        /// </summary>
        /// <param name="profilePic"></param>
        /// <param name="options"></param>
        /// <returns>The ticket of the upload, used for fetching the status later</returns>
        public async Task<string> UploadProfilePictureAsync(Stream profilePic, RequestOptions options = null)
        {
            string ticketResponse = (await Client.SendAsync("POST", "me/profile_image_url", options)).ReadAsString();
            string ticket = ticketResponse.Split('/').Last();
            await Client.SendStreamAsync("PUT", $"https://imageupload.gfycat.com/{ticket}","profilePic", profilePic, null, options);
            return ticket;
        }

        /// <summary>
        /// Gets the status of an uploading profile pic
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(string ticket, RequestOptions options = null)
        {
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), (await Client.SendAsync("GET", $"me/profile_image_url/status/{ticket}", options)).ReadAsString(), true);
        }

        /// <summary>
        /// Modifies the current user using the specified operations
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyCurrentUserAsync(IEnumerable<GfycatOperation> operations, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PATCH", "me", new { operations }, options);
        }

        /// <summary>
        /// Follows the specified user
        /// </summary>
        /// <param name="user">The user to follow</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task FollowUserAsync(IUser user, RequestOptions options = null)
        {
            return Client.SendAsync("PUT", $"me/follows/{user.Id}", options);
        }

        /// <summary>
        /// Unfollows the specified user
        /// </summary>
        /// <param name="user">The user to unfollow</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task UnfollowUserAsync(IUser user, RequestOptions options = null)
        {
            return Client.SendAsync("DELETE", $"me/follows/{user.Id}", options);
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

        /// <summary>
        /// Returns a full list of all Gfycats in the current users account, published or not.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cursor"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Client.SendAsync<GfycatFeed>("GET", $"me/gfycats{queryString}", options);
        }

        /// <summary>
        /// Returns a timeline list of all published Gfycats in the users you follow
        /// </summary>
        /// <param name="count">The number of Gfys to return</param>
        /// <param name="cursor">The cursor from the previous request, used to fetch the next page of gfys</param>
        /// <param name="options"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a folder for the current user using the specified name with a parent if specified
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="parent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task CreateFolderAsync(string folderName, string parent = null, RequestOptions options = null)
        {
            return Client.SendJsonAsync("POST", $"me/folders{(string.IsNullOrWhiteSpace(parent) ? "" : $"/{parent}")}", new { folderName }, options);
        }

        #endregion
        
        /// <summary>
        /// Gets a collection of all bookmark folders of the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<IEnumerable<GfycatBookmarkFolderInfo>> GetBookmarkFoldersAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<GfycatBookmarkFolderInfo>>("GET", "me/bookmark-folders", options);
        }

        #region Albums

        /// <summary>
        /// Get all album information for the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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

        public Task AddTwitterProviderAsync(string secret, RequestOptions options = null)
        {
            return Client.SendJsonAsync("POST", "me/providers", new { provider = "twitter", secret }, options);
        }
        
        public Task AddTwitterProviderAsync(string verifier, string token, RequestOptions options = null)
        {
            return Client.SendJsonAsync("POST", "me/providers", new { provider = "twitter", verifier, token }, options);
        }

        public Task RemoveTwitterProviderAsync(RequestOptions options = null)
        {
            return Client.SendAsync("DELETE", "me/providers/twitter", options);
        }
        
        public Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", $"me/domain-whitelist", options);
        }

        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/domain-whitelist", new { domainWhitelist = newWhitelist }, options);
            await UpdateAsync();
        }

        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/domain-whitelist", options);
            await UpdateAsync();
        }

        public Task<IEnumerable<string>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", $"me/geo-whitelist", options);
        }

        public async Task ModifyGeoWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/geo-whitelist", new { geoWhitelist = newWhitelist }, options);
            await UpdateAsync();
        }

        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/geo-whitelist", options);
            await UpdateAsync();
        }
    }
}
