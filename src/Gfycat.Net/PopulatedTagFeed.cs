using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.TrendingTagsFeed;

namespace Gfycat
{
    [DebuggerDisplay("Tag count: {Content.Count}")]
    public class PopulatedTagFeed : IFeed<TaggedGfyFeed>
    {
        readonly RequestOptions _defaultOptions;
        readonly GfycatClient _client;
        string IFeed<TaggedGfyFeed>.Cursor => _cursor;
        internal string _cursor { get; set; }

        internal PopulatedTagFeed(GfycatClient client,  RequestOptions defaultOptions)
        {
            _defaultOptions = defaultOptions;
            _client = client;
        }

        public IReadOnlyCollection<TaggedGfyFeed> Content { get; private set; }
        
        internal static PopulatedTagFeed Create(GfycatClient client, RequestOptions options, Model model)
        {
            return new PopulatedTagFeed(client, options)
            {
                Content = model.Tags.Select(t => TaggedGfyFeed.Create(client, t, options)).ToReadOnlyCollection(),
                _cursor = model.Cursor
            };
        }
        /// <summary>
        /// Returns an enumerator to enumerate through this feed
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerator<TaggedGfyFeed> GetEnumerator()
        {
            return new FeedEnumerator<TaggedGfyFeed>(_client, this, _defaultOptions);
        }

        public async Task<IFeed<TaggedGfyFeed>> GetNextPageAsync(RequestOptions options = null)
        {
            return Create(_client, options, await _client.ApiClient.GetTrendingTagsPopulatedAsync(_cursor, options));
        }
    }
}
