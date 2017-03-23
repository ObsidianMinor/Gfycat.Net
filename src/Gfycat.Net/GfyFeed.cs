using System.Collections.Generic;

namespace Gfycat
{
    public abstract class GfyFeed : IFeed<Gfy>
    {
        internal readonly GfycatClient _client;
        internal readonly int _count;
        internal readonly RequestOptions _options;

        internal GfyFeed(GfycatClient client, int defaultCount, RequestOptions defaultOptions)
        {
            _client = client;
            _count = defaultCount;
            _options = defaultOptions;
        }

        public IReadOnlyCollection<Gfy> Content { get; internal set; }

        public string Cursor { get; internal set; }

        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
