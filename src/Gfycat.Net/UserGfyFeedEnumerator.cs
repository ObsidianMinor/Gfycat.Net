using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class UserGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        readonly string _userId;

        internal UserGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, int count, RequestOptions defaultOptions, string userId) : base(client, feed, count, defaultOptions)
        {
            _userId = userId;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return UserGfyFeed.Create(_client, count, options, _userId, await _client.ApiClient.GetUserGfyFeedAsync(_userId, count, cursor, options));
        }
    }
}
