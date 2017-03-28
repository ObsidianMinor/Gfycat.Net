using System.Collections.Generic;
using System.Linq;
using Model = Gfycat.API.Models.TrendingFeed;

namespace Gfycat
{
    public class ReactionFeed : GfyFeed
    {
        public string Tag { get; private set; }

        internal ReactionFeed(GfycatClient client, RequestOptions options) : base(client, options)
        {
        }

        internal static ReactionFeed Create(GfycatClient client, Model trendingFeed, RequestOptions options)
        {
            return new ReactionFeed(client, options)
            {
                Content = trendingFeed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Tag = trendingFeed.Tag,
                Cursor = trendingFeed.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new SiteSearchEnumerator(_client, Tag, _options, this);
        }
    }
}
