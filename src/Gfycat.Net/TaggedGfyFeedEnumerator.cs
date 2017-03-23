using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class TaggedGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        readonly string /*thatguy*/tagg;

        public TaggedGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, int count, string tag, RequestOptions options) : base(client, feed, count, options)
        {
            tagg = tag;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return TaggedGfyFeed.Create(_client, count, (await _client.ApiClient.GetTrendingFeedAsync(tagg, count, cursor, options ?? _options)), options ?? _options);
        }
    }
}
