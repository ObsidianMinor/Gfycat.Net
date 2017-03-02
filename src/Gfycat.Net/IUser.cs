using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents a Gfycat user account
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// A unique identifier for the user
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The user’s username on Gfycat
        /// </summary>
        string Username { get; }
        /// <summary>
        /// The user’s profile description
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The user’s profile link
        /// </summary>
        string ProfileUrl { get; }
        /// <summary>
        /// The user’s name on Gfycat
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The number of user’s gfy views on Gfycat
        /// </summary>
        int Views { get; }
        /// <summary>
        /// The user’s email verification status
        /// </summary>
        bool EmailVerified { get; }
        /// <summary>
        /// The URL to the user’s profile on Gfycat
        /// </summary>
        string Url { get; }
        /// <summary>
        /// The date the user created their account
        /// </summary>
        DateTime CreationDate { get; }
        /// <summary>
        /// The URL to the user’s avatar on Gfycat
        /// </summary>
        string ProfileImageUrl { get; }
        /// <summary>
        /// The account’s verified status
        /// </summary>
        bool Verified { get; }
        /// <summary>
        /// The number of user’s followers
        /// </summary>
        int Followers { get; }
        /// <summary>
        /// The number of users this user follows
        /// </summary>
        int Following { get; }
        /// <summary>
        /// The user’s profile image visibility on the iframe
        /// </summary>
        bool IframeProfileImageVisible { get; }

        /// <summary>
        /// Get all album information for this user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync(RequestOptions options = null);

        /// <summary>
        /// Returns a full list of Gfycats that the user has published
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cursor"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null, RequestOptions options = null);
    }
}
