using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Defines a folder's content
    /// </summary>
    public interface IFolderContent : IFolder, IUpdatable
    {
        /// <summary>
        /// Gets the number of gfys in this folder
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the content of this folder
        /// </summary>
        IReadOnlyCollection<Gfy> Content { get; }
        /// <summary>
        /// Moves the specified gfys to another folder
        /// </summary>
        /// <param name="folderToMoveTo"></param>
        /// <param name="gfysToMove"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null);
    }
}
