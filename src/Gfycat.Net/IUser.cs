using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents a basic Gfycat user
    /// </summary>
    public interface IUser : IUpdatable
    {
        /// <summary>
        /// The username of this user
        /// </summary>
        string Username { get; }
        /// <summary>
        /// The description of this user
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Gets the URL provided on the user's profile
        /// </summary>
        string ProfileUrl { get; }
        /// <summary>
        /// Gets this user's name provided on their profile
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the total number of Gfy views this user has recieved
        /// </summary>
        int Views { get; }
        /// <summary>
        /// Gets a browser friendly URL to this user's profile
        /// </summary>
        string Url { get; }
        /// <summary>
        /// Gets the date and time of this user's account creation
        /// </summary>
        DateTime CreationDate { get; }
        /// <summary>
        /// Gets this user's profile image url
        /// </summary>
        string ProfileImageUrl { get; }
        /// <summary>
        /// Gets whether this user is verified
        /// </summary>
        bool Verified { get; }
        /// <summary>
        /// Gets the number of users following this user
        /// </summary>
        int Followers { get; }
        /// <summary>
        /// Gets the number of users this user is following
        /// </summary>
        int Following { get; }
        /// <summary>
        /// Gets the user’s profile image visibility on the iframe
        /// </summary>
        bool IframeProfileImageVisible { get; }
        /// <summary>
        /// Gets the number of Gfys this user has published on their account
        /// </summary>
        int PublishedGfys { get; }
        /// <summary>
        /// Gets the number of albums this user had published on their account
        /// </summary>
        int PublishedAlbums { get; }

        /// <summary>
        /// Gets the albums of this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IEnumerable<IAlbumInfo>> GetAlbumsAsync(RequestOptions options = null);
        /// <summary>
        /// Gets the feed of gfys for this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<GfyFeed> GetGfyFeedAsync(RequestOptions options = null);
        /// <summary>
        /// Follows this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task FollowAsync(RequestOptions options = null);
        /// <summary>
        /// Unfollows this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task UnfollowAsync(RequestOptions options = null);
        /// <summary>
        /// Gets if the current user is following this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> GetFollowingUser(RequestOptions options = null);
    }
}
