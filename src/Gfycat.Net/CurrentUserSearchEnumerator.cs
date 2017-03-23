using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserSearchEnumerator : FeedEnumerator<Gfy>
    {
        readonly string _searchText;

        internal CurrentUserSearchEnumerator(GfycatClient client, string searchText, RequestOptions options, IFeed<Gfy> feed, int count) : base(client, feed, count, options)
        {
            _searchText = searchText;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return CurrentUserSearchFeed.Create(_client, await _client.ApiClient.SearchCurrentUserAsync(_searchText, count, cursor, _options), _searchText, options ?? _options, _count);
        }
    }
}
