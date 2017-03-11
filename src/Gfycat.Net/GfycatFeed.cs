using System.Collections.Generic;
using System;
using Model = Gfycat.API.Models.Feed;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatFeed
    {
        readonly GfycatClient _client;
        public IReadOnlyCollection<Gfy> Gfycats { get; internal set; }
        public string Cursor { get; private set; }

        internal GfycatFeed(GfycatClient client)
        {
            _client = client;
        }

        internal static GfycatFeed Create(GfycatClient gfycatClient, Model feed)
        {
            GfycatFeed gfyFeed = new GfycatFeed(gfycatClient)
            {
                Gfycats = feed.Gfycats.Select(g => Gfy.Create(gfycatClient, g)).ToReadOnlyCollection(),
                Cursor = feed.Cursor
            };
            return gfyFeed;
        }

        public async Task<GfycatFeed> GetNextAsync(int count)
        {
            throw new NotImplementedException();
        }

        async Task<IFeed> IFeed.GetNextAsync(int count) => await GetNextAsync(count);
    }
}
