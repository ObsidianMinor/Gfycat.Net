using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IUser
    {
        string Id { get; }
        string Username { get; }
        string Description { get; }
        string ProfileUrl { get; }
        string Name { get; }
        int Views { get; }
        string Url { get; }
        DateTime CreationDate { get; }
        string ProfileImageUrl { get; }
        bool Verified { get; }
        int Followers { get; }
        int Following { get; }
        bool IframeProfileImageVisible { get; }
        int PublishedGfys { get; }
        int PublishedAlbums { get; }

        Task<IEnumerable<IAlbumInfo>> GetAlbumsAsync(RequestOptions options = null);
        Task<GfyFeed> GetGfyFeedAsync(RequestOptions options = null);
        Task FollowAsync(RequestOptions options = null);
        Task UnfollowAsync(RequestOptions options = null);
        Task<bool> GetFollowingUser(RequestOptions options = null);
    }
}
