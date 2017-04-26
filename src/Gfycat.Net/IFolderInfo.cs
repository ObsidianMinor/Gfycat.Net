using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Defines a folder with basic information in a folder tree
    /// </summary>
    public interface IFolderInfo : IFolder
    {
        /// <summary>
        /// Gets all folders inside this folder
        /// </summary>
        IReadOnlyCollection<IFolderInfo> Subfolders { get; }

        /// <summary>
        /// Gets the contents of this folder
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task<IFolderContent> GetContentsAsync(RequestOptions options = null);
        /// <summary>
        /// Moves this folder to somewhere else in the folder tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task MoveFolderAsync(IFolderInfo parent, RequestOptions options = null);
        /// <summary>
        /// Creates a new folder inside this folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task CreateNewFolderAsync(string folderName, RequestOptions options = null);
    }
}
