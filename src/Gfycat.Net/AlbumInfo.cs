using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.AlbumInfo;

namespace Gfycat
{
    /// <summary>
    /// Represents an album's basic information
    /// </summary>
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
                Subfolders = albumInfo.Nodes?.Select(a => Create(client, a, ownerId)).ToReadOnlyCollection(),
                Published = albumInfo.Published,
                CoverImageUrl = albumInfo.CoverImageUrl,
                CoverImageUrlMobile = albumInfo.CoverImageUrlMobile,
                Mp4Url = albumInfo.Mp4Url,
                WebmUrl = albumInfo.WebmUrl,
                GifUrl = albumInfo.GifUrl,
                MobileUrl = albumInfo.MobileUrl,
                MobilePosterUrl = albumInfo.MobilePosterUrl,
                PosterUrl = albumInfo.PosterUrl,
                Thumb360Url = albumInfo.Thumb360Url,
                Thumb360PosterUrl = albumInfo.Thumb360PosterUrl,
                Thumb100PosterUrl = albumInfo.Thumb100PosterUrl,
                Max5MbGif = albumInfo.Max5MbGif,
                Max2MbGif = albumInfo.Max2MbGif,
                MjpgUrl = albumInfo.MjpgUrl,
                Height = albumInfo.Height,
                Width = albumInfo.Width,
                MiniPosterUrl = albumInfo.MiniPosterUrl,
                MiniUrl = albumInfo.MiniUrl,
                WebpUrl = albumInfo.WebpUrl
            };
        }
        /// <summary>
        /// Gets the title of this Gfycat folder
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets all folders inside this folder
        /// </summary>
        public IReadOnlyCollection<IAlbumInfo> Subfolders { get; private set; }
        /// <summary>
        /// Gets whether this album is published or not
        /// </summary>
        public bool Published { get; private set; }
        /// <summary>
        /// Gets the cover image url for this album
        /// </summary>
        public string CoverImageUrl { get; private set; }
        /// <summary>
        /// Gets the mobile cover image url for this album
        /// </summary>
        public string CoverImageUrlMobile { get; private set; }
        /// <summary>
        /// Gets the mp4 url for the cover gfy for this album
        /// </summary>
        public string Mp4Url { get; private set; }
        /// <summary>
        /// Gets the webm url for the cover gfy for this album
        /// </summary>
        public string WebmUrl { get; private set; }
        /// <summary>
        /// Gets the webp url for the cover gfy for this album
        /// </summary>
        public string WebpUrl { get; private set; }
        /// <summary>
        /// Gets the gif url for the cover gfy for this album
        /// </summary>
        public string GifUrl { get; private set; }
        /// <summary>
        /// Gets the mobile url for this album
        /// </summary>
        public string MobileUrl { get; private set; }
        /// <summary>
        /// Gets the mobile poster url for this album
        /// </summary>
        public string MobilePosterUrl { get; private set; }
        /// <summary>
        /// Gets the poster url for this album
        /// </summary>
        public string PosterUrl { get; private set; }
        /// <summary>
        /// Gets the 360mb thumbnail url for this album
        /// </summary>
        public string Thumb360Url { get; private set; }
        /// <summary>
        /// Gets the 360mb thumbnail poster url for this album
        /// </summary>
        public string Thumb360PosterUrl { get; private set; }
        /// <summary>
        /// Gets the 100mb thumbnail poster url for this album
        /// </summary>
        public string Thumb100PosterUrl { get; private set; }
        /// <summary>
        /// Gets the max 5mb gif url
        /// </summary>
        public string Max5MbGif { get; private set; }
        /// <summary>
        /// Gets the max 2mb gif url
        /// </summary>
        public string Max2MbGif { get; private set; }
        /// <summary>
        /// Gets the mjpg url for this album
        /// </summary>
        public string MjpgUrl { get; private set; }
        /// <summary>
        /// Gets the mini url for this album
        /// </summary>
        public string MiniUrl { get; private set; }
        /// <summary>
        /// Gets the mini poster url for this album
        /// </summary>
        public string MiniPosterUrl { get; private set; }
        /// <summary>
        /// Gets the width of this album's cover gfy
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Gets the height of this album's cover gfy
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Gets the contents of this album
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<Album> GetContentsAsync(RequestOptions options = null)
        {
            return Album.Create(Client, (_owner == null) ? await Client.ApiClient.GetAlbumContentsAsync(Id, options).ConfigureAwait(false) : await Client.ApiClient.GetAlbumContentsAsync(_owner, Id, options).ConfigureAwait(false));
        }
        /// <summary>
        /// Moves this album to another location in the folder tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task MoveFolderAsync(IAlbumInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveAlbumAsync(Id, parent.Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Creates a new album folder inside this album
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumInFolderAsync(Id, folderName, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Creates a new album inside this album
        /// </summary>
        /// <param name="albumName"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task CreateNewAlbumAsync(string albumName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumAsync(Id, albumName, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Changes the title of this folder to the provided string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes this folder on Gfycat
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options).ConfigureAwait(false);
        }

        #region IFolderInfo explicits

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) => await GetContentsAsync(options).ConfigureAwait(false);
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo parent, RequestOptions options) => await MoveFolderAsync(parent as IAlbumInfo ?? throw new ArgumentException(), options).ConfigureAwait(false);
        
        #endregion
    }
}
