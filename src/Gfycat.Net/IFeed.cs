using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents a page in a feed of content from Gfycat
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFeed<T> : IAsyncEnumerable<T>
    {
        /// <summary>
        /// Contains the current page of content for this feed
        /// </summary>
        IReadOnlyCollection<T> Content { get; }
        /// <summary>
        /// The cursor used to get the next feed
        /// </summary>
        string Cursor { get; }
        /// <summary>
        /// Returns the next page of the current feed
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IFeed<T>> GetNextPageAsync(RequestOptions options = null);
    }
}
