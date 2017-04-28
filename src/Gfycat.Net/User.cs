using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.User;

namespace Gfycat
{
    /// <summary>
    /// Represents a user on Gfycat
    /// </summary>
    [DebuggerDisplay("Username: {Username}")]
    public class User : Entity, IUser
    {
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
        /// Gets the number of Gfys this user has published on their account
        /// </summary>
        public int PublishedGfys { get; private set; }
        /// <summary>
        /// Gets the number of albums this user had published on their account
        /// </summary>
        public int PublishedAlbums { get; private set; }

        internal User(GfycatClient client, string id) : base(client, id)
        { }

        internal void Update(Model model)
        {
            CreationDate = model.CreationDate;
            Description = model.Description;
            Followers = model.Followers;
            Following = model.Following;
            IframeProfileImageVisible = model.IframeProfileImageVisible;
            Name = model.Name;
            ProfileImageUrl = model.ProfileImageUrl;
            ProfileUrl = model.ProfileUrl;
            Url = model.Url;
            Username = model.Username;
            Verified = model.Verified;
            Views = model.Views;
            PublishedGfys = model.PublishedGfycats;
            PublishedAlbums = model.PublishedAlbums;
        }

        internal static User Create(GfycatClient client, Model model)
        {
            User user = new User(client, model.Id);
            user.Update(model);
            return user;
        }
        /// <summary>
        /// Updates this object with the latest server information
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UpdateAsync(RequestOptions options = null)
        {
            Update(await Client.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false));
        }
        /// <summary>
        /// Returns all public albums for this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlbumInfo>> GetAlbumsAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetAlbumsForUserAsync(Id, options).ConfigureAwait(false)).Select(album => AlbumInfo.Create(Client, album, Id));
        }

        async Task<IEnumerable<IAlbumInfo>> IUser.GetAlbumsAsync(RequestOptions options) => await GetAlbumsAsync(options).ConfigureAwait(false);
        /// <summary>
        /// Returns the public gfy feed for this user
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<GfyFeed> GetGfyFeedAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, Client.ApiClient.Config.DefaultFeedItemCount);
            return UserGfyFeed.Create(Client, count, options, Id, await Client.ApiClient.GetUserGfyFeedAsync(Id, count, null, options).ConfigureAwait(false));
        }
        /// <summary>
        /// Follows this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task FollowAsync(RequestOptions options = null)
        {
            await Client.ApiClient.FollowUserAsync(Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Unfollows this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UnfollowAsync(RequestOptions options = null)
        {
            await Client.ApiClient.UnfollowUserAsync(Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets whether the current user is following this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> GetFollowingUser(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowingUserAsync(Id, options).ConfigureAwait(false)) == System.Net.HttpStatusCode.OK;
        }
    }
}
