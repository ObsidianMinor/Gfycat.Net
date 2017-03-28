using System.Collections.Generic;
using System.Linq;
using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    internal class CurrentUserSearchFeed : GfyFeed
    {
        readonly string _searchText;

        internal CurrentUserSearchFeed(GfycatClient client, string searchText, RequestOptions options) : base(client, options)
        {
            _searchText = searchText;
        }

        internal static CurrentUserSearchFeed Create(GfycatClient client, Model model, string searchText, RequestOptions options)
        {
            return new CurrentUserSearchFeed(client, searchText, options)
            {
                Content = model.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = model.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new CurrentUserSearchEnumerator(_client, _searchText, _options, this);
        }
    }
}
