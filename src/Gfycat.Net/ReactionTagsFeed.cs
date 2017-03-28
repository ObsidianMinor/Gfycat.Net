using System.Collections.Generic;
using System.Linq;
using Model = Gfycat.API.Models.TrendingTagsFeed;

namespace Gfycat
{
    public class ReactionTagsFeed : IFeed<ReactionFeed>
    {
        readonly RequestOptions _defaultOptions;
        readonly GfycatClient _client;
        readonly ReactionLanguage _language;

        internal ReactionTagsFeed(GfycatClient client, RequestOptions defaultOptions, ReactionLanguage language)
        {
            _defaultOptions = defaultOptions;
            _client = client;
            _language = language;
        }

        public IReadOnlyCollection<ReactionFeed> Content { get; private set; }

        public string Cursor { get; private set; }

        internal static ReactionTagsFeed Create(GfycatClient client, RequestOptions options, Model model, ReactionLanguage lang)
        {
            return new ReactionTagsFeed(client, options, lang)
            {
                Content = model.Tags.Select(t => ReactionFeed.Create(client, t, options)).ToReadOnlyCollection(),
                Cursor = model.Cursor
            };
        }

        public IAsyncEnumerator<ReactionFeed> GetEnumerator()
        {
            return new ReactionTagsFeedEnumerator(_client, this, _defaultOptions, _language);
        }
    }
}
