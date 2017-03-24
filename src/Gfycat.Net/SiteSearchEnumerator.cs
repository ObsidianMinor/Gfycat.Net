using System.Threading.Tasks;

namespace Gfycat
{
    internal class SiteSearchEnumerator : FeedEnumerator<Gfy>
    {
        private string _searchText;

        internal SiteSearchEnumerator(GfycatClient client, string searchText, RequestOptions options, IFeed<Gfy> siteSearchFeed) : base(client, siteSearchFeed, options)
        {
            _searchText = searchText;
        }

        protected override async Task<IFeed<Gfy>> GetNext(string cursor, RequestOptions options = null)
        {
            return SiteSearchFeed.Create(_client, await _client.ApiClient.SearchSiteAsync(_searchText, cursor, options), _searchText, options);
        }
    }
}