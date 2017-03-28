using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class ReactionTagsFeedEnumerator : FeedEnumerator<ReactionFeed>
    {
        readonly ReactionLanguage _lang;

        public ReactionTagsFeedEnumerator(GfycatClient client, IFeed<ReactionFeed> feed, RequestOptions defaultOptions, ReactionLanguage lang) : base(client, feed, defaultOptions)
        {
            _lang = lang;
        }

        protected async override Task<IFeed<ReactionFeed>> GetNext(string cursor, RequestOptions options = null)
        {
            return ReactionTagsFeed.Create(_client, options, await _client.ApiClient.GetReactionGifsPopulatedAsync(_lang, cursor, options), _lang);
        }
    }
}
