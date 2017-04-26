using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.CurrentUser;

namespace Gfycat
{
    /// <summary>
    /// Represents the current login user on Gfycat
    /// </summary>
    [DebuggerDisplay("Username: {Username}")]
    public class CurrentUser : Entity, IUser, ISearchable
    {
        private Uri _currentUploadUrl;

        internal CurrentUser(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            Username = model.Username;
            Description = model.Description;
            ProfileUrl = model.ProfileUrl;
            Name = model.Name;
            Views = model.Views;
            EmailVerified = model.EmailVerified;
            Url = model.Url;
            CreationDate = model.CreationDate;
            ProfileImageUrl = model.ProfileImageUrl;
            Verified = model.Verified;
            Followers = model.Followers;
            Following = model.Following;
            IframeProfileImageVisible = model.IframeProfileImageVisible;
            GeoWhitelist = model.GeoWhitelist.Select(s => new RegionInfo(s)).ToReadOnlyCollection();
            DomainWhitelist = model.DomainWhitelist.ToReadOnlyCollection();
            Email = model.Email;
            UploadNotices = model.UploadNotices;
            TotalGfys = model.TotalGfycats;
            TotalAlbums = model.TotalAlbums;
            TotalBookmarks = model.TotalBookmarks;
            PublishedGfys = model.PublishedGfycats;
            PublishedAlbums = model.PublishedAlbums;
        }

        /// <summary>
        /// Updates this object with the latest server information
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetCurrentUserAsync(options).ConfigureAwait(false));
        
        internal static CurrentUser Create(GfycatClient gfycatClient, Model currentUserModel)
        {
            CurrentUser user = new CurrentUser(gfycatClient, currentUserModel.Id);
            user.Update(currentUserModel);
            return user;
        }
        
        /// <summary>
        /// The username of this user
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// The description of this user
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Gets the URL provided on the user's profile
        /// </summary>
        public string ProfileUrl { get; private set; }
        /// <summary>
        /// Gets this user's name provided on their profile
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the total number of Gfy views this user has recieved
        /// </summary>
        public int Views { get; private set; }
        /// <summary>
        /// Gets whether this user's email is verified
        /// </summary>
        public bool EmailVerified { get; private set; }
        /// <summary>
        /// Gets a browser friendly URL to this user's profile
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// Gets the date and time of this user's account creation
        /// </summary>
        public DateTime CreationDate { get; private set; }
        /// <summary>
        /// Gets this user's profile image url
        /// </summary>
        public string ProfileImageUrl { get; private set; }
        /// <summary>
        /// Gets whether this user is verified
        /// </summary>
        public bool Verified { get; private set; }
        /// <summary>
        /// Gets the number of users following this user
        /// </summary>
        public int Followers { get; private set; }
        /// <summary>
        /// Gets the number of users this user is following
        /// </summary>
        public int Following { get; private set; }
        /// <summary>
        /// Gets the user’s profile image visibility on the iframe
        /// </summary>
        public bool IframeProfileImageVisible { get; private set; }
        /// <summary>
        /// The user’s geo whitelist on Gfycat
        /// </summary>
        public IReadOnlyCollection<RegionInfo> GeoWhitelist { get; private set; }
        /// <summary>
        /// The user’s domain whitelist on Gfycat
        /// </summary>
        public IReadOnlyCollection<string> DomainWhitelist { get; private set; }
        /// <summary>
        /// The email address of the specified user
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// The whether the user recieves upload notices when a gfy has finished uploading
        /// </summary>
        public bool UploadNotices { get; private set; }
        /// <summary>
        /// Gets the number of Gfys this user has published on their account
        /// </summary>
        public int PublishedGfys { get; private set; }
        /// <summary>
        /// Gets the number of albums this user had published on their account
        /// </summary>
        public int PublishedAlbums { get; private set; }
        /// <summary>
        /// Gets the number of gfys this user has on their account
        /// </summary>
        public int TotalGfys { get; private set; }
        /// <summary>
        /// Gets the number of gfys this user has bookmarked on their account
        /// </summary>
        public int TotalBookmarks { get; private set; }
        /// <summary>
        /// Gets the number of albums this user has on their account
        /// </summary>
        public int TotalAlbums { get; private set; }

        #region Users

        /// <summary>
        /// Gets whether the current user's email is verified
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns>True if the current user's email is verified, otherwise false</returns>
        public async Task<bool> GetEmailVerifiedAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetEmailVerificationStatusAsync(options).ConfigureAwait(false)) != HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Sends a verification email to the current user's email address
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task SendVerificationEmailAsync(RequestOptions options = null)
        {
            await Client.ApiClient.SendVerificationEmailRequestAsync(options).ConfigureAwait(false);
        }

        /// <summary>
        /// Modifies the current user's profile pic
        /// </summary>
        /// <param name="profilePic"></param>
        /// <param name="options">The options for this request</param>
        public async Task UploadProfilePictureAsync(Stream profilePic, RequestOptions options = null)
        {
            _currentUploadUrl = new Uri(await Client.ApiClient.GetProfileUploadUrlAsync(options).ConfigureAwait(false));
            await Client.ApiClient.UploadProfileImageAsync(_currentUploadUrl.ToString(), profilePic, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the status of an uploading profile pic
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(RequestOptions options = null)
        {
            if (_currentUploadUrl == null)
                throw new InvalidOperationException("The current upload url isn't set! Have you uploaded a profile pic yet?");

            var status = await Client.ApiClient.GetProfileImageUploadStatusAsync(_currentUploadUrl.Segments.LastOrDefault(), options).ConfigureAwait(false);
            if (status == ProfileImageUploadStatus.Succeeded)
                await UpdateAsync().ConfigureAwait(false);

            return status;
        }

        /// <summary>
        /// Modifies the current user using the specified operations
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task ModifyCurrentUserAsync(IEnumerable<GfycatOperation> operations, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyCurrentUserAsync(operations, options).ConfigureAwait(false);
            await UpdateAsync(options).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns an enumerable user ids of the users the current user is following
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetFollowingUsersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowingsAsync(options).ConfigureAwait(false)).Follows;
        }

        /// <summary>
        /// Returns an enumerable of users following the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetFollowersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowersAsync(options).ConfigureAwait(false)).Followers;
        }

        /// <summary>
        /// Returns an enumerable of the users the current user is following
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetFollowingUsersPopulatedAsync(RequestOptions options = null)
        {
            return await Task.WhenAll((await GetFollowingUsersAsync(options).ConfigureAwait(false)).Select(following => Client.GetUserAsync(following, options))).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an enumerable of the users following the current user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetFollowersPopulatedAsync(RequestOptions options = null)
        {
            return await Task.WhenAll((await GetFollowersAsync(options)).Select(following => Client.GetUserAsync(following, options))).ConfigureAwait(false);
        }

        #endregion

        #region User feeds

        /// <summary>
        /// Gets the current user's private gfy feed
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyFeed> GetGfyFeedAsync(RequestOptions options = null)
        {
            return CurrentUserGfyFeed.Create(Client, options, await Client.ApiClient.GetCurrentUserGfyFeedAsync(null, options).ConfigureAwait(false));
        }

        /// <summary>
        /// Returns a timeline list of all published gfys in the users this user follows
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyFeed> GetTimelineFeedAsync(RequestOptions options = null)
        {
            return CurrentUserTimelineFeed.Create(Client, options, await Client.ApiClient.GetFollowsGfyFeedAsync(null, options).ConfigureAwait(false));
        }

        #endregion

        #region User Folders

        /// <summary>
        /// Retrieves a list of folder information for the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FolderInfo>> GetFoldersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetCurrentUserFoldersAsync(options)).Select(folder => FolderInfo.Create(Client, folder));
        }

        /// <summary>
        /// Creates a folder for the current user using the specified name with a parent if specified
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task CreateFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateFolderAsync(null, folderName, options).ConfigureAwait(false);
        }

        #endregion
        
        /// <summary>
        /// Gets a collection of all bookmark folders of the current user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<BookmarkFolderInfo>> GetBookmarkFoldersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetCurrentUserBookmarkFoldersAsync(options).ConfigureAwait(false)).Select(folder => BookmarkFolderInfo.Create(Client, folder));
        }

        #region Albums

        /// <summary>
        /// Get all album information for the current user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<IAlbumInfo>> GetAlbumsAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetAlbumsAsync(options).ConfigureAwait(false)).Select(album => Utils.CreateAlbum(Client, album, null));
        }
        
        #endregion

        /// <summary>
        /// Searches the current user's gfys
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyFeed> SearchAsync(string searchText, RequestOptions options = null)
        {
            return CurrentUserSearchFeed.Create(Client, await Client.ApiClient.SearchCurrentUserAsync(searchText, null, options).ConfigureAwait(false), searchText, options);
        }
        /// <summary>
        /// Adds a twitter provider to this account using the specified verifier and token
        /// </summary>
        /// <param name="verifier"></param>
        /// <param name="token"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task AddTwitterProviderAsync(string verifier, string token, RequestOptions options = null)
        {
            await Client.ApiClient.AddProviderAsync(new API.AddProviderParameters() { Provider = "twitter", Token = token, Verifier = verifier }, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Removes the current user's twitter provider
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task RemoveTwitterProviderAsync(RequestOptions options = null)
        {
            await Client.ApiClient.RemoveProviderAsync("twitter", options).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets the whitelist of domains allowed to embed this user's content
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetDomainWhitelistAsync(options).ConfigureAwait(false)).DomainWhitelist;
        }
        /// <summary>
        /// Changes the whitelist of domains allowed to embed this user's content to the new whitelist
        /// </summary>
        /// <param name="newWhitelist"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyDomainWhitelistAsync(new API.DomainWhitelistShared { DomainWhitelist = newWhitelist }, options).ConfigureAwait(false);
            await UpdateAsync(options).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes the whitelist of domains allowed to embed this user's content
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteDomainWhitelistAsync(options).ConfigureAwait(false);
            await UpdateAsync(options).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets the whitelist of regions allowed to embed this user's content
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<RegionInfo>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetGeoWhitelistAsync(options).ConfigureAwait(false)).GeoWhitelist.Select(s => new RegionInfo(s));
        }
        /// <summary>
        /// Changes the whitelist of regions allowed to embed this user's content to the new whitelist
        /// </summary>
        /// <param name="newWhitelist"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task ModifyGeoWhitelistAsync(IEnumerable<RegionInfo> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGeoWhitelistAsync(new API.GeoWhitelistShared { GeoWhitelist = newWhitelist.Select(r => r.TwoLetterISORegionName) }, options).ConfigureAwait(false);
            await UpdateAsync(options).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes the whitelist of regions allowed to embed this user's content
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGeoWhitelistAsync(options).ConfigureAwait(false);
            await UpdateAsync(options).ConfigureAwait(false);
        }

        #region API Credentials

        /// <summary>
        /// Fetches the developer keys for the current user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<AppApiInfo>> GetApiCredentialsAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetDeveloperKeysAsync(options).ConfigureAwait(false)).Select(a => AppApiInfo.Create(Client, a));
        }

        #endregion

        /// <summary>
        /// Follows this user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        async Task IUser.FollowAsync(RequestOptions options)
        {
            await Client.ApiClient.FollowUserAsync(Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Unfollows this user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        async Task IUser.UnfollowAsync(RequestOptions options)
        {
            await Client.ApiClient.UnfollowUserAsync(Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets whether the current user is following this user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        async Task<bool> IUser.GetFollowingUser(RequestOptions options)
        {
            return (await Client.ApiClient.GetFollowingUserAsync(Id, options).ConfigureAwait(false)) == HttpStatusCode.OK;
        }
    }
}
