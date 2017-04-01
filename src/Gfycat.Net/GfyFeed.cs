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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly GfycatClient _client;
        internal readonly RequestOptions _options;
        string IFeed<Gfy>.Cursor => _cursor;
        internal string _cursor { get; set; }

        internal GfyFeed(GfycatClient client, RequestOptions defaultOptions)
        {
            _client = client;
            _options = defaultOptions;
        }

        public IReadOnlyCollection<Gfy> Content { get; internal set; }

        public abstract Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null);

        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
