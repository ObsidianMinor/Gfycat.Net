using System.Diagnostics;

namespace Gfycat
{
    /// <summary>
    /// Represents a search feed with a given count
    /// </summary>
    [DebuggerDisplay("Search: {SearchText}")]
    public abstract class SearchFeed : GfyFeed
    {
        internal SearchFeed(GfycatClient client, string searchText, int count, RequestOptions defaultOptions) : base(client, count, defaultOptions)
        {
            SearchText = searchText;
        }

        /// <summary>
        /// Gets the search text of this feed
        /// </summary>
        public string SearchText { get; }

        /// <summary>
        /// Gets the total number of Gfys found in the search
        /// </summary>
        public int Count { get; internal set; }
    }
}
