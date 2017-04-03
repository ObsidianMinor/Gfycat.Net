using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents a folder of albums
    /// </summary>
    public class AlbumFolder : Entity, IAlbumInfo
    {
        internal AlbumFolder(GfycatClient client, string id) : base(client, id)
        {
        }

        internal static AlbumFolder Create(GfycatClient client, API.Models.AlbumInfo model, string ownerId)
        {
            return new AlbumFolder(client, model.Id)
            {
                Title = model.Title,
                Subfolders = model.Nodes.Select(a => Utils.CreateAlbum(client, a, ownerId)).ToReadOnlyCollection(),
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
        bool IAlbumInfo.Published => false;

        Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) => throw new NotSupportedException();
        /// <summary>
        /// Moves this album to another location in the folder tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveFolderAsync(IAlbumInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveAlbumAsync(Id, parent.Id, options);
        }
        /// <summary>
        /// Creates a new album inside of this folder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateNewAlbumAsync(string name, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumInFolderAsync(Id, name, options);
        }
        /// <summary>
        /// Creates a new album folder inside of this folder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateNewFolderAsync(string name, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumAsync(Id, name, options);
        }
        /// <summary>
        /// Changes the title of this folder to the provided string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options);
        }
        /// <summary>
        /// Deletes this folder on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options);
        }

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo folderInfo, RequestOptions options) => await MoveFolderAsync(folderInfo as IAlbumInfo ?? throw new ArgumentException(), options);
    }
}
