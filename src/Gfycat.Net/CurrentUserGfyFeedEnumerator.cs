using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        internal CurrentUserGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, RequestOptions defaultOptions) : base(client, feed, defaultOptions)
        {
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return CurrentUserGfyFeed.Create(_client, _options, await _client.ApiClient.GetCurrentUserGfyFeedAsync(cursor, options));
        }
    }
}
