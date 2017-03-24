using System.Collections.Generic;

namespace Gfycat
{
    public abstract class GfyFeed : IFeed<Gfy>
    {
        internal readonly GfycatClient _client;
        internal readonly RequestOptions _options;

        internal GfyFeed(GfycatClient client, RequestOptions defaultOptions)
        {
            _client = client;
            _options = defaultOptions;
        }

        public IReadOnlyCollection<Gfy> Content { get; internal set; }

        public string Cursor { get; internal set; }

        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
