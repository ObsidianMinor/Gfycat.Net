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
    public class CurrentUser : Entity, IUser, IUpdatable, ISearchable
    {
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

        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetCurrentUserAsync(options));
        
        internal static CurrentUser Create(GfycatClient gfycatClient, Model currentUserModel)
        {
            CurrentUser user = new CurrentUser(gfycatClient, currentUserModel.Id);
            user.Update(currentUserModel);
            return user;
        }
        
        public string Username { get; private set; }

        public string Description { get; private set; }

        public string ProfileUrl { get; private set; }

        public string Name { get; private set; }

        public int Views { get; private set; }

        public bool EmailVerified { get; private set; }

        public string Url { get; private set; }

        public DateTime CreationDate { get; private set; }

        public string ProfileImageUrl { get; private set; }

        public bool Verified { get; private set; }

        public int Followers { get; private set; }

        public int Following { get; private set; }

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
        /// The user’s upload notices settings, whether the user wants to get notified of uploads or not
        /// </summary>
        public bool UploadNotices { get; private set; }

        public int PublishedGfys { get; private set; }

        public int PublishedAlbums { get; private set; }

        public int TotalGfys { get; private set; }

        public int TotalBookmarks { get; private set; }

        public int TotalAlbums { get; private set; }

        #region Users

        /// <summary>
        /// Gets whether the current user's email is verified
        /// </summary>
        /// <param name="options"></param>
        /// <returns>True if the current user's email is verified, otherwise false</returns>
        public async Task<bool> GetEmailVerifiedAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetEmailVerificationStatusAsync(options)) != HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Sends a verification email to the current user's email address
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendVerificationEmailAsync(RequestOptions options = null)
        {
            await Client.ApiClient.SendVerificationEmailRequestAsync(options);
        }

        /// <summary>
        /// Modifies the current user's profile pic
        /// </summary>
        /// <param name="profilePic"></param>
        /// <param name="options"></param>
        public async Task UploadProfilePictureAsync(Stream profilePic, RequestOptions options = null)
        {
            await Client.ApiClient.UploadProfileImageAsync(await Client.ApiClient.GetProfileUploadUrlAsync(options), profilePic, options);
        }

        /// <summary>
        /// Gets the status of an uploading profile pic
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(RequestOptions options = null)
        {
            Uri ticket = new Uri(await Client.ApiClient.GetProfileUploadUrlAsync(options));
            return await Client.ApiClient.GetProfileImageUploadStatusAsync(ticket.AbsolutePath.Trim('/'), options);
        }

        /// <summary>
        /// Modifies the current user using the specified operations
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyCurrentUserAsync(IEnumerable<GfycatOperation> operations, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyCurrentUserAsync(operations, options);
            await UpdateAsync(options);
        }
        
        /// <summary>
        /// Returns an enumerable user ids of the users the current user is following
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetFollowingUsersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowingsAsync(options)).Follows;
        }

        /// <summary>
        /// Returns an enumerable of users following the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetFollowersAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowersAsync(options)).Followers;
        }

        #endregion

        #region User feeds

        public async Task<GfyFeed> GetGfyFeedAsync(RequestOptions options = null)
        {
            return CurrentUserGfyFeed.Create(Client, options, await Client.ApiClient.GetCurrentUserGfyFeedAsync(null, options));
        }

        /// <summary>
        /// Returns a timeline list of all published Gfycats in the users you follow
        /// </summary>
        /// <param name="count">The number of Gfys to return</param>
        /// <param name="cursor">The cursor from the previous request, used to fetch the next page of gfys</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<GfyFeed> GetTimelineFeedAsync(RequestOptions options = null)
        {
            return CurrentUserTimelineFeed.Create(Client, options, await Client.ApiClient.GetFollowsGfyFeedAsync(null, options));
        }

        #endregion

        #region User Folders

        /// <summary>
        /// Retrieves a list of folder information for the current user
        /// </summary>
        /// <returns></returns>
        public async Task<FolderInfo> GetFoldersAsync(RequestOptions options = null)
        {
            return FolderInfo.Create(Client, (await Client.ApiClient.GetCurrentUserFoldersAsync(options)).FirstOrDefault()); // skip the first one,
        }

        /// <summary>
        /// Creates a folder for the current user using the specified name with a parent if specified
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="parent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateFolderAsync(null, folderName, options);
        }

        #endregion
        
        /// <summary>
        /// Gets a collection of all bookmark folders of the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<BookmarkFolderInfo> GetBookmarkFoldersAsync(RequestOptions options = null)
        {
            return BookmarkFolderInfo.Create(Client, (await Client.ApiClient.GetCurrentUserBookmarkFoldersAsync(options)).FirstOrDefault());
        }

        #region Albums

        /// <summary>
        /// Get all album information for the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IAlbumInfo> GetAlbumsAsync(RequestOptions options = null)
        {
            return Utils.CreateAlbum(Client, (await Client.ApiClient.GetAlbumsAsync(options)).FirstOrDefault(), null);
        }
        
        #endregion

        public async Task<GfyFeed> SearchAsync(string searchText, RequestOptions options = null)
        {
            return CurrentUserSearchFeed.Create(Client, await Client.ApiClient.SearchCurrentUserAsync(searchText, null, options), searchText, options);
        }

        public async Task AddTwitterProviderAsync(string secret, RequestOptions options = null)
        {
            await Client.ApiClient.AddProviderAsync(new API.AddProviderParameters() { Provider = "twitter", Secret = secret }, options);
        }
        
        public async Task AddTwitterProviderAsync(string verifier, string token, RequestOptions options = null)
        {
            await Client.ApiClient.AddProviderAsync(new API.AddProviderParameters() { Provider = "twitter", Token = token, Verifier = verifier }, options);
        }

        public async Task RemoveTwitterProviderAsync(RequestOptions options = null)
        {
            await Client.ApiClient.RemoveProviderAsync("twitter", options);
        }
        
        public async Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetDomainWhitelistAsync(options)).DomainWhitelist;
        }

        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyDomainWhitelistAsync(new API.DomainWhitelistShared { DomainWhitelist = newWhitelist }, options);
            await UpdateAsync(options);
        }

        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteDomainWhitelistAsync(options);
            await UpdateAsync(options);
        }

        public async Task<IEnumerable<RegionInfo>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetGeoWhitelistAsync(options)).GeoWhitelist.Select(s => new RegionInfo(s));
        }

        public async Task ModifyGeoWhitelistAsync(IEnumerable<RegionInfo> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGeoWhitelistAsync(new API.GeoWhitelistShared { GeoWhitelist = newWhitelist.Select(r => r.TwoLetterISORegionName) }, options);
            await UpdateAsync(options);
        }

        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGeoWhitelistAsync(options);
            await UpdateAsync(options);
        }

        #region API Credentials

        /// <summary>
        /// Fetches the developer keys for the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AppApiInfo>> GetApiCredentialsAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetDeveloperKeysAsync(options)).Select(a => AppApiInfo.Create(Client, a));
        }

        #endregion

        Task IUser.FollowAsync(RequestOptions options) => throw new NotSupportedException("You can't follow yourself");

        Task IUser.UnfollowAsync(RequestOptions options) => throw new NotSupportedException("You can't unfollow yourself");

        Task<bool> IUser.GetFollowingUser(RequestOptions options) => Task.FromResult(false);
    }
}
