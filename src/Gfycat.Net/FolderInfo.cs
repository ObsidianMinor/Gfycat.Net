using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.FolderInfo;

namespace Gfycat
{
    /// <summary>
    /// Represents a folder's basic info
    /// </summary>
    public class FolderInfo : Entity, IFolderInfo
    {
        internal FolderInfo(GfycatClient client, string id) : base(client, id)
        {
        }

        internal static FolderInfo Create(GfycatClient client, Model model)
        {
            return new FolderInfo(client, model.Id)
            {
                Subfolders = model.Subfolders.Select(f => Create(client, f)).ToReadOnlyCollection(),
                Title = model.Title
            };
        }

        /// <summary>
        /// Gets the title of this Gfycat folder
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets all folders inside this folder
        /// </summary>
        public IReadOnlyCollection<FolderInfo> Subfolders { get; private set; }
        /// <summary>
        /// Gets the contents of this album
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<Folder> GetContentsAsync(RequestOptions options = null)
        {
            return Folder.Create(Client, await Client.ApiClient.GetFolderContentsAsync(Id, options).ConfigureAwait(false));
        }
        /// <summary>
        /// Moves this album to another location in the folder tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveFolderAsync(FolderInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveFolderAsync(Id, parent.Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Creates a new folder inside of this folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateFolderAsync(Id, folderName, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Changes the title of this folder to the provided string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyFolderTitleAsync(Id, newTitle, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes this folder on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteFolderAsync(Id, options).ConfigureAwait(false);
        }

        #region Explicit IFolderInfo

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;

        async Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) => await GetContentsAsync(options).ConfigureAwait(false);
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo parent, RequestOptions options) => await MoveFolderAsync(parent as FolderInfo ?? throw new ArgumentException(), options).ConfigureAwait(false);

        #endregion
    }
}
