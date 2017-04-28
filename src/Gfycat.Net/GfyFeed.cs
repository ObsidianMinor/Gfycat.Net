using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Gfycat
{
    /// <summary>
    /// Represents a basic gfy feed
    /// </summary>
    public abstract class GfyFeed : IFeed<Gfy>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] // I don't want to see the client everywhere I go
        internal readonly GfycatClient _client;
        internal readonly RequestOptions _options;
        string IFeed<Gfy>.Cursor => _cursor;
        internal string _cursor { get; set; }
        internal readonly int _count;

        internal GfyFeed(GfycatClient client, int count, RequestOptions defaultOptions)
        {
            _count = count;
            _client = client;
            _options = defaultOptions;
        }
        /// <summary>
        /// Contains the current page of content for this feed
        /// </summary>
        public IReadOnlyCollection<Gfy> Content { get; internal set; }
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count">The number of gfys to get in this request</param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public abstract Task<IFeed<Gfy>> GetNextPageAsync(int count = 10, RequestOptions options = null);
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
