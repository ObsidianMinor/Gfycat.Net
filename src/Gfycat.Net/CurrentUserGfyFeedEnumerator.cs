using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        internal CurrentUserGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, int count, RequestOptions defaultOptions) : base(client, feed, count, defaultOptions)
        {
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return CurrentUserGfyFeed.Create(_client, _count, _options, await _client.ApiClient.GetCurrentUserGfyFeedAsync(count, cursor, options));
        }
    }
}
