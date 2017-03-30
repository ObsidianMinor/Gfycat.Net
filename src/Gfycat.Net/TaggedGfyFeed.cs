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
        public string Tag { get; private set; }

        internal TaggedGfyFeed(GfycatClient client, RequestOptions options) : base(client, options)
        {
        }
        
        internal static TaggedGfyFeed Create(GfycatClient client, Model trendingFeed, RequestOptions options)
        {
            return new TaggedGfyFeed(client, options)
            {
                Content = trendingFeed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Tag = trendingFeed.Tag,
                _cursor = trendingFeed.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }

        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, await _client.ApiClient.GetTrendingFeedAsync(Tag, _cursor, options), options);
        }
    }
}
