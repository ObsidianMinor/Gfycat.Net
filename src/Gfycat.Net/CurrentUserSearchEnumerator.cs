using System;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class CurrentUserSearchEnumerator : FeedEnumerator<Gfy>
    {
        readonly GfycatClient _client;
        readonly string _searchText;
        readonly RequestOptions _options;

        internal CurrentUserSearchEnumerator(GfycatClient client, string searchText, RequestOptions options, IFeed<Gfy> feed, int count) : base(feed, count)
        {
            _client = client;
            _searchText = searchText;
            _options = options;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count)
        {
            return GfyFeed.Create(_client, (await _client.ApiClient.SearchCurrentUserAsync(_searchText, count, cursor, _options)));
        }
    }
}
