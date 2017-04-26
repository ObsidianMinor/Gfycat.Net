using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Defines a basic Gfycat folder with an id and title
    /// </summary>
    public interface IFolder
    {
        /// <summary>
        /// Gets the id of this Gfycat folder
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the title of this Gfycat folder
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Changes the title of this folder to the provided string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task ModifyTitleAsync(string newTitle, RequestOptions options = null);

        /// <summary>
        /// Deletes this folder on Gfycat
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task DeleteAsync(RequestOptions options = null);
    }
}
