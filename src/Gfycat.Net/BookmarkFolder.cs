using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Folder;

namespace Gfycat
{
    /// <summary>
    /// Represents a bookmark folder including content
    /// </summary>
    public class BookmarkFolder : Entity, IFolderContent, IUpdatable
    {
        internal BookmarkFolder(GfycatClient client, string id) : base(client, id)
        {
        }

        void Update(Model model)
        {
            Title = model.Title;
            Count = model.GfyCount;
            Content = model.PublishedGfys.Select(g => Gfy.Create(Client, g)).ToReadOnlyCollection();
        }

        internal static BookmarkFolder Create(GfycatClient client, Model folder)
        {
            BookmarkFolder bookmarkFolder = new BookmarkFolder(client, folder.Id);
            bookmarkFolder.Update(folder);
            return bookmarkFolder;
        }
        /// <summary>
        /// Gets the title of this bookmark folder
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets the number of gfys in this bookmark folder
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Gets the content of this bookmark folder
        /// </summary>
        public IReadOnlyCollection<Gfy> Content { get; private set; }
        /// <summary>
        /// Modifies the title of this gfy to the new value
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyBookmarkFolderTitleAsync(Id, newTitle, options);
            await UpdateAsync(options);
        }
        /// <summary>
        /// Moves the specified gfys to another bookmark folder
        /// </summary>
        /// <param name="folderToMoveTo"></param>
        /// <param name="gfysToMove"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveGfysAsync(BookmarkFolder folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            await Client.ApiClient.MoveBookmarkedGfysAsync(Id, new API.GfyFolderAction() { Action = "move_contents", GfycatIds = gfysToMove.Select(g => g.Id), ParentId = folderToMoveTo.Id }, options);
            await UpdateAsync(options);
            await folderToMoveTo.UpdateAsync();
        }
        /// <summary>
        /// Deletes this bookmark folder on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteBookmarkFolderAsync(Id, options);
        }
        /// <summary>
        /// Updates this bookmark folder using the latest info from the server
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetBookmarkFolderContentsAsync(Id, options));

        #region Explicit IFolder
        
        Task IFolderContent.MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options)
            => MoveGfysAsync(folderToMoveTo as BookmarkFolder ?? throw new ArgumentException($"Parent folder isn't a bookmark folder", nameof(folderToMoveTo)), gfysToMove, options);
        
        #endregion
    }
}
