using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.SearchFeed;

namespace Gfycat
{
    internal class SiteSearchFeed : SearchFeed
    {
        internal SiteSearchFeed(GfycatClient client, int count, string searchText, RequestOptions options) : base(client, searchText, count, options)
        {
        }

        internal static SiteSearchFeed Create(GfycatClient client, Model model, int count, string searchText, RequestOptions options)
        {
            return new SiteSearchFeed(client, count, searchText, options)
            {
                Content = model.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = model.Cursor,
                Count = model.Found
            };
        }
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override Task<IFeed<Gfy>> GetNextPageAsync(int count = 10, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, _count);
            return Create(_client, await _client.ApiClient.SearchSiteAsync(SearchText, count, _cursor, options).ConfigureAwait(false), count, SearchText, options);
        }
    }
}
