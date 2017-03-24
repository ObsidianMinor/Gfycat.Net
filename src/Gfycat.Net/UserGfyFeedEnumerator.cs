using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class UserGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        readonly string _userId;

        internal UserGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, RequestOptions defaultOptions, string userId) : base(client, feed, defaultOptions)
        {
            _userId = userId;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return UserGfyFeed.Create(_client, options, _userId, await _client.ApiClient.GetUserGfyFeedAsync(_userId, cursor, options));
        }
    }
}
