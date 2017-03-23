using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.AlbumInfo;

namespace Gfycat
{
    public class AlbumInfo : Entity, IAlbumInfo
    {
        readonly string _owner;

        internal AlbumInfo(GfycatClient client, string id, string owner) : base(client, id)
        {
            _owner = owner;
        }
        
        internal static AlbumInfo Create(GfycatClient client, Model albumInfo, string ownerId)
        {
            return new AlbumInfo(client, albumInfo.Id, ownerId)
            {
                Title = albumInfo.Title,
                Subfolders = albumInfo.Nodes.Select(a => Create(client, a, ownerId)).ToReadOnlyCollection(),
                Published = albumInfo.Published,
            };
        }

        public string Title { get; private set; }
        public IReadOnlyCollection<IAlbumInfo> Subfolders { get; private set; }
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
            return Album.Create(Client, (_owner == null) ? await Client.ApiClient.GetAlbumContentsAsync(Id, options) : await Client.ApiClient.GetAlbumContentsAsync(_owner, Id, options));
        }

        public async Task MoveFolderAsync(IAlbumInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveAlbumAsync(Id, parent.Id, options);
        }

        public async Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumInFolderAsync(Id, folderName, options);
        }

        public async Task CreateNewAlbumAsync(string albumName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumAsync(Id, albumName, options);
        }

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options);
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options);
        }

        #region IFolderInfo explicits

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) => await GetContentsAsync(options);
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo parent, RequestOptions options) => await MoveFolderAsync(parent as IAlbumInfo ?? throw new ArgumentException(), options);
        
        #endregion
    }
}
