using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
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
                Published = model.Published
            };
        }

        public string Title { get; private set; }
        public IReadOnlyCollection<IAlbumInfo> Subfolders { get; private set; }
        public bool Published { get; private set; }

        Task<IFolderContent> IFolderInfo.GetContentsAsync(RequestOptions options) => throw new NotSupportedException();

        public async Task MoveFolderAsync(IAlbumInfo parent, RequestOptions options = null)
        {
            await Client.ApiClient.MoveAlbumAsync(Id, parent.Id, options);
        }

        public async Task CreateNewAlbumAsync(string name, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumInFolderAsync(Id, name, options);
        }

        public async Task CreateNewFolderAsync(string name, RequestOptions options = null)
        {
            await Client.ApiClient.CreateAlbumAsync(Id, name, options);
        }

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options);
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options);
        }

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;
        async Task IFolderInfo.MoveFolderAsync(IFolderInfo folderInfo, RequestOptions options) => await MoveFolderAsync(folderInfo as IAlbumInfo ?? throw new ArgumentException(), options);
    }
}
