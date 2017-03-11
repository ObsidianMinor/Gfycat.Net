using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    public class SearchFeed : IFeed
    {
        private readonly GfycatClient _client;
        private readonly bool _isMeSearch;
        private readonly int _afterCount;

        public IReadOnlyCollection<Gfy> Gfycats { get; internal set; }
        public string Cursor { get; internal set; }
        public int Found { get; internal set; }

        internal SearchFeed(GfycatClient client, bool isMeSearch, int afterCount)
        {
            _isMeSearch = isMeSearch;
            _client = client;
            _afterCount = afterCount;
        }

        internal static SearchFeed Create(GfycatClient client, bool meSearch, int afterCount, Model model)
        {
            return new SearchFeed(client, meSearch, afterCount)
            {
                Cursor = model.Cursor,
                Gfycats = model.Gfycats.Select(gm => Gfy.Create(client, gm)).ToReadOnlyCollection()
            };
        }

        public async Task<SearchFeed> GetNextAsync(int count, RequestOptions options)
        {
            Model result = (_isMeSearch) ? await _client.ApiClient.SearchCurrentUserAsync()
        }

        async Task<IFeed> IFeed.GetNextAsync(int count, RequestOptions options) => await GetNextAsync(count, options);
    }
}