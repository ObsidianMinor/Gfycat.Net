using System.Collections.Generic;
using System.Linq;
using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    internal class SiteSearchFeed : GfyFeed
    {
        readonly string _searchText;

        internal SiteSearchFeed(GfycatClient client, string searchText, RequestOptions options) : base(client, options)
        {
            _searchText = searchText;
        }

        internal static CurrentUserSearchFeed Create(GfycatClient client, Model model, string searchText, RequestOptions options)
        {
            return new CurrentUserSearchFeed(client, searchText, options)
            {
                Content = model.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                Cursor = model.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new SiteSearchEnumerator(_client, _searchText, _options, this);
        }
    }
}
