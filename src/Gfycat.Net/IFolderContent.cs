using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFolderContent : IFolder
    {
        int Count { get; }
        IReadOnlyCollection<Gfy> Content { get; }

        Task MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null);
    }
}
