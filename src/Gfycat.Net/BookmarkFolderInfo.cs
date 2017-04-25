using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.FolderInfo;

namespace Gfycat
{
    /// <summary>
    /// Represents a bookmark folder's basic info
    /// </summary>
    public class BookmarkFolderInfo : Entity, IFolderInfo
    {
        internal BookmarkFolderInfo(GfycatClient client, string id) : base(client, id)
        {
        }

        internal static BookmarkFolderInfo Create(GfycatClient client, Model model)
        {
            return new BookmarkFolderInfo(client, model.Id)
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
        public IReadOnlyCollection<BookmarkFolderInfo> Subfolders { get; private set; }
        /// <summary>
        /// Gets the contents of this album
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<BookmarkFolder> GetContentsAsync(RequestOptions options = null)
        {
            return BookmarkFolder.Create(Client, await Client.ApiClient.GetBookmarkFolderContentsAsync(Id, options).ConfigureAwait(false));
        }
        /// <summary>
        /// Moves this album to another location in the folder tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveFolderAsync(BookmarkFolderInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveBookmarkFolderAsync(Id, parent.Id, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Creates a new folder inside of this folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateBookmarkFolderAsync(Id, folderName, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Changes the title of this folder to the provided string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyBookmarkFolderTitleAsync(Id, newTitle, options).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes this folder on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteBookmarkFolderAsync(Id, options).ConfigureAwait(false);
        }

        #region Explicit IFolderInfo

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) 
            => await GetContentsAsync(options).ConfigureAwait(false);
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo parent, RequestOptions options) => await MoveFolderAsync(parent as BookmarkFolderInfo ?? throw new ArgumentException(), options).ConfigureAwait(false);

        #endregion
    }
}
