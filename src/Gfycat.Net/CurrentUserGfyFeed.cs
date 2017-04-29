using System;
using System.Collections.Generic;
using System.Text;
using Gfycat.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserGfyFeed : GfyFeed
    {
        internal CurrentUserGfyFeed(GfycatClient client, RequestOptions defaultOptions) : base(client, defaultOptions)
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

        internal static CurrentUserGfyFeed Create(GfycatClient client, RequestOptions options, Feed feed)
        {
            return new CurrentUserGfyFeed(client, options)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = feed.Cursor
            };
        }
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, options, await _client.ApiClient.GetCurrentUserGfyFeedAsync(_cursor, options).ConfigureAwait(false));
        }
    }
}
