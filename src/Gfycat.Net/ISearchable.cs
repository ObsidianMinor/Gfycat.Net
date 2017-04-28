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
        /// <param name="count"></param>
        /// <param name="searchText"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task<SearchFeed> SearchAsync(string searchText, int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null);
    }
}
