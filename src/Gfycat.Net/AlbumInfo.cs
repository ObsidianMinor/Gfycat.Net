using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.AlbumInfo;

namespace Gfycat
{
    public class AlbumInfo : Entity, IFolderInfo
    {
        internal AlbumInfo(GfycatClient client, string id) : base(client, id)
        {
        }

        internal static AlbumInfo Create(GfycatClient client, Model albumInfo)
        {
            return new AlbumInfo(client, albumInfo.Id)
            {
                Title = albumInfo.Title,
                Subfolders = albumInfo.Nodes.Select(a => Create(client, a)).ToReadOnlyCollection(),
                Published = albumInfo.Published,
            };
        }

        public string Title { get; private set; }
        public IReadOnlyCollection<AlbumInfo> Subfolders { get; private set; }
        public bool Published { get; private set; }
        public string CoverImageUrl { get; private set; }
        public string CoverImageUrlMobile { get; private set; }
        public string Mp4Url { get; private set; }
        public string WebmUrl { get; private set; }
        public string WebpUrl { get; private set; }
        public string GifUrl { get; private set; }
        public string MobileUrl { get; private set; }
        public string MobilePosterUrl { get; private set; }
        public string PosterUrl { get; private set; }
        public string Thumb360Url { get; private set; }
        public string Thumb360PosterUrl { get; private set; }
        public string Thumb100PosterUrl { get; private set; }
        public string Max5MbGif { get; private set; }
        public string Max2MbGif { get; private set; }
        public string MjpgUrl { get; private set; }
        public string MiniUrl { get; private set; }
        public string MiniPosterUrl { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public async Task<Album> GetContentsAsync(RequestOptions options = null)
        {
            return Album.Create(Client, await Client.ApiClient.GetAlbumContentsAsync(Id, options));
        }

        #region IFolderInfo explicits

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task<IFolder> IFolderInfo.GetContentsAsync(RequestOptions options) => await GetContentsAsync(options);
        
        #endregion
    }
}
