using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Feed;

namespace Gfycat
{
    /// <summary>
    /// Represents a collection of gfys based on a particular reaction
    /// </summary>
    public class ReactionFeed : GfyFeed
    {
        /// <summary>
        /// Gets the cover gfy of this reaction feed
        /// </summary>
        public Gfy CoverGfy => Content.FirstOrDefault();
        /// <summary>
        /// Gets the tag of this reaction feed
        /// </summary>
        public string Tag { get; }

        internal ReactionFeed(GfycatClient client, int count, string searchText, RequestOptions options) : base(client, count, options)
        {
            Tag = searchText;
        }

        internal static ReactionFeed Create(GfycatClient client, int count, Model model, string searchText, RequestOptions options)
        {
            return new ReactionFeed(client, count, searchText, options)
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
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override Task<IFeed<Gfy>> GetNextPageAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, _count);

            return SiteSearchFeed.Create(_client, await _client.ApiClient.SearchSiteAsync(Tag, count, null, options).ConfigureAwait(false), count, Tag, options);
        }
    }
}
