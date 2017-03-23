using System;
using System.Collections.Generic;
using System.Text;
using Gfycat.API.Models;
using System.Linq;

namespace Gfycat
{
    internal class CurrentUserGfyFeed : GfyFeed
    {
        internal CurrentUserGfyFeed(GfycatClient client, int defaultCount, RequestOptions defaultOptions) : base(client, defaultCount, defaultOptions)
        {
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new CurrentUserGfyFeedEnumerator(_client, this, _count, _options);
        }

        internal static CurrentUserGfyFeed Create(GfycatClient client, int count, RequestOptions options, Feed feed)
        {
            return new CurrentUserGfyFeed(client, count, options)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Cursor = feed.Cursor
            };
        }
    }
}
