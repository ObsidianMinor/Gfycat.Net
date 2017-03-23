using System.Collections.Generic;
using System.Linq;
using Model = Gfycat.API.Models.TrendingFeed;

namespace Gfycat
{
    /// <summary>
    /// A feed of popular <see cref="Gfy"/>s which share a tag
    /// </summary>
    public class TaggedGfyFeed : GfyFeed
    {
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
                Cursor = trendingFeed.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new TaggedGfyFeedEnumerator(_client, this, 10, Tag, _options);
        }
    }
}
