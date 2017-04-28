using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.TrendingFeed;

namespace Gfycat
{
    /// <summary>
    /// A feed of popular <see cref="Gfy"/>s which share a tag
    /// </summary>
    [DebuggerDisplay("Tag: {Tag}")]
    public class TaggedGfyFeed : GfyFeed
    {
        /// <summary>
        /// Gets the tag of this feed
        /// </summary>
        public string Tag { get; private set; }

        internal TaggedGfyFeed(GfycatClient client, int count, RequestOptions options) : base(client, count, options)
        {
        }
        
        internal static TaggedGfyFeed Create(GfycatClient client, int count, Model trendingFeed, RequestOptions options)
        {
            return new TaggedGfyFeed(client, count, options)
            {
                Content = trendingFeed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Tag = trendingFeed.Tag,
                _cursor = trendingFeed.Cursor
            };
        }
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }
        /// <summary>
        /// Returns the next page of this gfy feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override Task<IFeed<Gfy>> GetNextPageAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, _count);
            return Create(_client, count, await _client.ApiClient.GetTrendingFeedAsync(Tag, count, _cursor, options).ConfigureAwait(false), options);
        }
    }
}
