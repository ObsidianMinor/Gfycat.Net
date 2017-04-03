using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Exposes a search method for searching the gfycat website or the current user
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Searches this type
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<GfyFeed> SearchAsync(string searchText, RequestOptions options = null);
    }
}
