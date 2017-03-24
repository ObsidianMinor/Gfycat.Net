using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserSearchEnumerator : FeedEnumerator<Gfy>
    {
        readonly string _searchText;

        internal CurrentUserSearchEnumerator(GfycatClient client, string searchText, RequestOptions options, IFeed<Gfy> feed) : base(client, feed, options)
        {
            _searchText = searchText;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return CurrentUserSearchFeed.Create(_client, await _client.ApiClient.SearchCurrentUserAsync(_searchText, cursor, _options), _searchText, options ?? _options);
        }
    }
}
