using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class PopulatedTagFeedEnumerator : FeedEnumerator<TaggedGfyFeed>
    {
        readonly int _defaultGfyCount;

        internal PopulatedTagFeedEnumerator(GfycatClient client, IFeed<TaggedGfyFeed> feed, int count, int gfyCount, RequestOptions defaultOptions) : base(client, feed, count, defaultOptions)
        {
            _defaultGfyCount = gfyCount;
        }

        protected override async Task<IFeed<TaggedGfyFeed>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return PopulatedTagFeed.Create(_client, _defaultGfyCount, count, options, await _client.ApiClient.GetTrendingTagsPopulatedAsync(count, _defaultGfyCount, cursor, options));
        }
    }
}
