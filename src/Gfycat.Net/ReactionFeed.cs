using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    public class ReactionFeed : GfyFeed
    {
        public Gfy CoverGfy => Content.FirstOrDefault();
        public string Tag { get; }

        internal ReactionFeed(GfycatClient client, string searchText, RequestOptions options) : base(client, options)
        {
            Tag = searchText;
        }

        internal static ReactionFeed Create(GfycatClient client, Model model, string searchText, RequestOptions options)
        {
            return new ReactionFeed(client, searchText, options)
            {
                Content = model.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection()
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

        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return SiteSearchFeed.Create(_client, await _client.ApiClient.SearchSiteAsync(Tag, null, options), Tag, options);
        }
    }
}
