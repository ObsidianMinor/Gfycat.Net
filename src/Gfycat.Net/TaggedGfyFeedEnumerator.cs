using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class TaggedGfyFeedEnumerator : FeedEnumerator<Gfy>
    {
        readonly string /*thatguy*/tagg;

        public TaggedGfyFeedEnumerator(GfycatClient client, IFeed<Gfy> feed, string tag, RequestOptions options) : base(client, feed, options)
        {
            tagg = tag;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return TaggedGfyFeed.Create(_client, (await _client.ApiClient.GetTrendingFeedAsync(tagg, cursor, options ?? _options)), options ?? _options);
        }
    }
}
