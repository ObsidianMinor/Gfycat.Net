using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }

        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, await _client.ApiClient.SearchCurrentUserAsync(_searchText, _cursor, options), _searchText, options);
        }
    }
}
