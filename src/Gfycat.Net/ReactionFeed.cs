using System.Collections.Generic;
using System.Linq;

using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    public class ReactionFeed : GfyFeed
    {
        public string Tag { get; }

        internal ReactionFeed(GfycatClient client, string searchText, RequestOptions options) : base(client, options)
        {
            Tag = searchText;
        }

        internal static ReactionFeed Create(GfycatClient client, Model model, string searchText, RequestOptions options)
        {
            return new ReactionFeed(client, searchText, options)
            {
                Content = model.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection()
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new SiteSearchEnumerator(_client, Tag, _options, this);
        }
    }
}
