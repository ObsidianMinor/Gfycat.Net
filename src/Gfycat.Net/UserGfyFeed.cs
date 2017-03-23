using System;
using System.Collections.Generic;
using System.Linq;

namespace Gfycat
{
    internal class UserGfyFeed : GfyFeed
    {
        readonly string _userId;

        internal UserGfyFeed(GfycatClient client, int defaultCount, RequestOptions defaultOptions, string userId) : base(client, defaultCount, defaultOptions)
        {
            _userId = userId;
        }

        internal static UserGfyFeed Create(GfycatClient client, int defaultCount, RequestOptions defaultOptions, string userId, API.Models.Feed feed)
        {
            return new UserGfyFeed(client, defaultCount, defaultOptions, userId)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Cursor = feed.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new UserGfyFeedEnumerator(_client, this, _count, _options, _userId);
        }
    }
}
