using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.FolderInfo;

namespace Gfycat
{
    public class FolderInfo : Entity, IFolderInfo
    {
        public FolderInfo(GfycatClient client, string id) : base(client, id)
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

        public string Title { get; private set; }

        public IReadOnlyCollection<FolderInfo> Subfolders { get; private set; }

        public async Task<Folder> GetContentsAsync(RequestOptions options = null)
        {
            return Folder.Create(Client, await Client.ApiClient.GetFolderContentsAsync(Id, options));
        }

        #region Explicit IFolderInfo

        IReadOnlyCollection<IFolderInfo> IFolderInfo.Subfolders => Subfolders;

        async Task<IFolder> IFolderInfo.GetContentsAsync(RequestOptions options) => await GetContentsAsync(options);

        #endregion
    }
}
