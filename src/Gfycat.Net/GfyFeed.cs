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

        internal GfyFeed(GfycatClient client, RequestOptions defaultOptions)
        {
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
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null);
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
