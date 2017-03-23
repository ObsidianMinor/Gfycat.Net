using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserTimelineFeedEnumerator : FeedEnumerator<Gfy>
    {
        internal CurrentUserTimelineFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, int count, RequestOptions defaultOptions) : base(client, feed, count, defaultOptions)
        {
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return CurrentUserTimelineFeed.Create(_client, count, options, await _client.ApiClient.GetFollowsGfyFeedAsync(count, cursor, options));
        }
    }
}
