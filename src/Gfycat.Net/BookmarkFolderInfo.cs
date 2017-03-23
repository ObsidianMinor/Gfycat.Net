using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.FolderInfo;

namespace Gfycat
{
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

        public string Title { get; private set; }

        public IReadOnlyCollection<BookmarkFolderInfo> Subfolders { get; private set; }

        public async Task<BookmarkFolder> GetContentsAsync(RequestOptions options = null)
        {
            return BookmarkFolder.Create(Client, await Client.ApiClient.GetBookmarkFolderContentsAsync(Id, options));
        }

        public async Task MoveFolderAsync(BookmarkFolderInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveBookmarkFolderAsync(Id, parent.Id, options);
        }

        public async Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            await Client.ApiClient.CreateBookmarkFolderAsync(Id, folderName, options);
        }

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyBookmarkFolderTitleAsync(Id, newTitle, options);
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteBookmarkFolderAsync(Id, options);
        }

        #region Explicit IFolderInfo

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) 
            => await GetContentsAsync(options);
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo parent, RequestOptions options) => await MoveFolderAsync(parent as BookmarkFolderInfo ?? throw new ArgumentException(), options);

        #endregion
    }
}
