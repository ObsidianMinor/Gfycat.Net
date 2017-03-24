using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserTimelineFeedEnumerator : FeedEnumerator<Gfy>
    {
        internal CurrentUserTimelineFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, RequestOptions defaultOptions) : base(client, feed, defaultOptions)
        {
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return CurrentUserTimelineFeed.Create(_client, options, await _client.ApiClient.GetFollowsGfyFeedAsync(cursor, options));
        }
    }
}
