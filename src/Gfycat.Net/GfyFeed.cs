using System;
using System.Collections.Generic;

using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    public abstract class GfyFeed : IFeed<Gfy>
    {
        protected readonly GfycatClient _client;

        protected GfyFeed(GfycatClient client)
        {
            _client = client;
        }

        public IReadOnlyCollection<Gfy> Content { get; private set; }

        public string Cursor { get; private set; }

        internal static IFeed<Gfy> Create(GfycatClient client, Model feed)
        {
            throw new NotImplementedException();
        }

        public abstract IAsyncEnumerator<Gfy> GetEnumerator();
    }
}
