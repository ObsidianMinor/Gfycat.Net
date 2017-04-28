using System.Collections.Generic;
using Gfycat.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserTimelineFeed : GfyFeed
    {
        internal CurrentUserTimelineFeed(GfycatClient client, int count, RequestOptions defaultOptions) : base(client, count, defaultOptions)
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

        internal static CurrentUserTimelineFeed Create(GfycatClient client, int count, RequestOptions defaultOptions, Feed feed)
        {
            return new CurrentUserTimelineFeed(client, count, defaultOptions)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = feed.Cursor
            };
        }
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override Task<IFeed<Gfy>> GetNextPageAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, _count);
            return Create(_client, count, options, await _client.ApiClient.GetFollowsGfyFeedAsync(count, _cursor, options).ConfigureAwait(false));
        }
    }
}
