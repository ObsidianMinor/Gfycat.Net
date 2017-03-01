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

        Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync(RequestOptions options = null);

        Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null, RequestOptions options = null);
    }
}
