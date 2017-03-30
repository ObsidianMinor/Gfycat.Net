using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class UserGfyFeed : GfyFeed
    {
        readonly string _userId;

        internal UserGfyFeed(GfycatClient client, RequestOptions defaultOptions, string userId) : base(client, defaultOptions)
        {
            _userId = userId;
        }

        internal static UserGfyFeed Create(GfycatClient client, RequestOptions defaultOptions, string userId, API.Models.Feed feed)
        {
            return new UserGfyFeed(client, defaultOptions, userId)
            {
                Content = feed.Gfycats.Select(g => Gfy.Create(client, g)).ToReadOnlyCollection(),
                _cursor = feed.Cursor
            };
        }

        public override IAsyncEnumerator<Gfy> GetEnumerator()
        {
            return new FeedEnumerator<Gfy>(_client, this, _options);
        }

        public async override Task<IFeed<Gfy>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, options, _userId, await _client.ApiClient.GetUserGfyFeedAsync(_userId, _cursor, options));
        }
    }
}
