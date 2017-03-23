using System.Threading.Tasks;

namespace Gfycat
{
    internal class SiteSearchEnumerator : FeedEnumerator<Gfy>
    {
        private string _searchText;

        internal SiteSearchEnumerator(GfycatClient client, string searchText, RequestOptions options, IFeed<Gfy> siteSearchFeed, int count) : base(client, siteSearchFeed, count, options)
        {
            _searchText = searchText;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, int count, RequestOptions options = null)
        {
            return SiteSearchFeed.Create(_client, await _client.ApiClient.SearchSiteAsync(_searchText, count, cursor, options), _searchText, options, count);
        }
    }
}