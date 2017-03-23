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
        bool EmailVerified { get; }
        string Url { get; }
        DateTime CreationDate { get; }
        string ProfileImageUrl { get; }
        bool Verified { get; }
        int Followers { get; }
        int Following { get; }
        bool IframeProfileImageVisible { get; }

        Task<IAlbumInfo> GetAlbumsAsync(RequestOptions options = null);
        Task<GfyFeed> GetGfycatFeedAsync(int count = 10, RequestOptions options = null);
        Task FollowAsync(RequestOptions options = null);
        Task UnfollowAsync(RequestOptions options = null);
        Task<bool> GetFollowingUser(RequestOptions options = null);
    }
}
