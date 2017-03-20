using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFolderInfo
    {
        string Id { get; }
        string Title { get; }
        IReadOnlyCollection<IFolderInfo> Subfolders { get; }

        Task<IFolder> GetContentsAsync(RequestOptions options = null);
    }
}
