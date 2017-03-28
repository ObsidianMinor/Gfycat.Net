using System.Collections.Generic;
using Gfycat.API.Models;
using System.Linq;

namespace Gfycat
{
    internal class CurrentUserTimelineFeed : GfyFeed
    {
        internal CurrentUserTimelineFeed(GfycatClient client, RequestOptions defaultOptions) : base(client, defaultOptions)
        {
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new CurrentUserTimelineFeedEnumerator(_client, this, _options);
        }

        internal static CurrentUserTimelineFeed Create(GfycatClient client, RequestOptions defaultOptions, Feed feed)
        {
            return new CurrentUserTimelineFeed(client, defaultOptions)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = feed.Cursor
            };
        }
    }
}
