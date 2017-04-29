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

        internal ReactionTagsFeed(GfycatClient client, RequestOptions defaultOptions, ReactionLanguage language)
        {
            _defaultOptions = defaultOptions;
            _client = client;
            _language = language;
        }

        /// <summary>
        /// The content on the current page of this feed
        /// </summary>
        public IReadOnlyCollection<ReactionFeed> Content { get; private set; }
        
        internal static ReactionTagsFeed Create(GfycatClient client, RequestOptions options, Model model, ReactionLanguage lang)
        {
            return new ReactionTagsFeed(client, options, lang)
            {
                Content = model.Tags.Select(t => ReactionFeed.Create(client, t, t.Tag, options)).ToReadOnlyCollection(),
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
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IFeed<ReactionFeed>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, options, await _client.ApiClient.GetReactionGifsPopulatedAsync(_language, _cursor, options).ConfigureAwait(false), _language);
        }
    }
}
