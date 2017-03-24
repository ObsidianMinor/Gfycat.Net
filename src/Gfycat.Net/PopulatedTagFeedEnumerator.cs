using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class PopulatedTagFeedEnumerator : FeedEnumerator<TaggedGfyFeed>
    {
        internal PopulatedTagFeedEnumerator(GfycatClient client, IFeed<TaggedGfyFeed> feed, RequestOptions defaultOptions) : base(client, feed, defaultOptions)
        {
        }

        protected override async Task<IFeed<TaggedGfyFeed>> GetNext(string cursor, RequestOptions options = null)
        {
            return PopulatedTagFeed.Create(_client, options, await _client.ApiClient.GetTrendingTagsPopulatedAsync(cursor, options));
        }
    }
}
