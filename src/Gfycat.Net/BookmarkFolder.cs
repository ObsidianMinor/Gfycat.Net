using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Folder;

namespace Gfycat
{
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

        public string Title { get; private set; }

        public int Count { get; private set; }

        public IReadOnlyCollection<Gfy> Content { get; private set; }
        
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyBookmarkFolderTitleAsync(Id, newTitle, options);
            await UpdateAsync(options);
        }

        public async Task MoveGfysAsync(BookmarkFolder folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            await Client.ApiClient.MoveBookmarkedGfysAsync(Id, new API.GfyFolderAction() { Action = "", GfycatIds = gfysToMove.Select(g => g.Id), ParentId = folderToMoveTo.Id }, options);
            await UpdateAsync(options);
            await folderToMoveTo.UpdateAsync();
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteBookmarkFolderAsync(Id, options);
        }

        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetBookmarkFolderContentsAsync(Id, options));

        #region Explicit IFolder
        
        Task IFolderContent.MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options)
            => MoveGfysAsync(folderToMoveTo as BookmarkFolder ?? throw new ArgumentException($"Parent folder isn't a bookmark folder", nameof(folderToMoveTo)), gfysToMove, options);
        
        #endregion
    }
}
