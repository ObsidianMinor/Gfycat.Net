using System.Collections.Generic;
using Gfycat.API.Models;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserTimelineFeed : GfyFeed
    {
        internal CurrentUserTimelineFeed(GfycatClient client, RequestOptions defaultOptions) : base(client, defaultOptions)
        {
        }
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }

        internal static CurrentUserTimelineFeed Create(GfycatClient client, RequestOptions defaultOptions, Feed feed)
        {
            return new CurrentUserTimelineFeed(client, defaultOptions)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = feed.Cursor
            };
        }

        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, options, await _client.ApiClient.GetFollowsGfyFeedAsync(_cursor, options));
        }
    }
}
