using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFolder
    {
        string Id { get; }
        string Title { get; }
        int Count { get; }
        IReadOnlyCollection<Gfy> Content { get; }

        Task ModifyTitleAsync(string newTitle, RequestOptions options = null);
        Task MoveFolderAsync(IFolder parent, RequestOptions options = null);
        Task MoveGfysAsync(IFolder folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null);
        Task CreateNewFolderAsync(string folderName, RequestOptions options = null);
    }
}
