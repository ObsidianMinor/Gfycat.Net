using System.Collections.Generic;
using Gfycat.API.Models;
using System.Linq;

namespace Gfycat
{
    internal class CurrentUserTimelineFeed : GfyFeed
    {
        internal CurrentUserTimelineFeed(GfycatClient client, int defaultCount, RequestOptions defaultOptions) : base(client, defaultCount, defaultOptions)
        {
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new CurrentUserTimelineFeedEnumerator(_client, this, _count, _options);
        }

        internal static CurrentUserTimelineFeed Create(GfycatClient client, int defaultCount, RequestOptions defaultOptions, Feed feed)
        {
            return new CurrentUserTimelineFeed(client, defaultCount, defaultOptions)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Cursor = feed.Cursor
            };
        }
    }
}
