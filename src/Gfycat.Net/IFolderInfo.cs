using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFolderInfo : IFolder
    {
        IReadOnlyCollection<IFolderInfo> Subfolders { get; }

        Task<IFolderContent> GetContentsAsync(RequestOptions options = null);
        Task MoveFolderAsync(IFolderInfo parent, RequestOptions options = null);
        Task CreateNewFolderAsync(string folderName, RequestOptions options = null);
    }
}
