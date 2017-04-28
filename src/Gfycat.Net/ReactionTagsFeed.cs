using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.TrendingTagsFeed;

namespace Gfycat
{
    /// <summary>
    /// Represents a collection of reaction feeds from Gfycat
    /// </summary>
    public class ReactionTagsFeed : IFeed<ReactionFeed>
    {
        readonly RequestOptions _defaultOptions;
        readonly GfycatClient _client;
        readonly ReactionLanguage _language;
        string IFeed<ReactionFeed>.Cursor => _cursor;
        internal string _cursor { get; set; }
        readonly int _count;

        internal ReactionTagsFeed(GfycatClient client, int count, RequestOptions defaultOptions, ReactionLanguage language)
        {
            _count = count;
            _defaultOptions = defaultOptions;
            _client = client;
            _language = language;
        }

        /// <summary>
        /// The content on the current page of this feed
        /// </summary>
        public IReadOnlyCollection<ReactionFeed> Content { get; private set; }
        
        internal static ReactionTagsFeed Create(GfycatClient client, int count, RequestOptions options, Model model, ReactionLanguage lang)
        {
            return new ReactionTagsFeed(client, count, options, lang)
            {
                Content = model.Tags.Select(t => ReactionFeed.Create(client, count, t, t.Tag, options)).ToReadOnlyCollection(),
                _cursor = model.Cursor
            };
        }

        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerator<ReactionFeed> GetEnumerator()
        {
            return new FeedEnumerator<ReactionFeed>(_client, this, _defaultOptions);
        }
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IFeed<ReactionFeed>> GetNextPageAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, _count);

            return Create(_client, count, options, await _client.ApiClient.GetReactionGifsPopulatedAsync(_language, count, _cursor, options).ConfigureAwait(false), _language);
        }
    }
}
