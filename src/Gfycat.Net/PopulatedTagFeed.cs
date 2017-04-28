using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.TrendingTagsFeed;

namespace Gfycat
{
    /// <summary>
    /// Represents a feed of <see cref="TaggedGfyFeed"/>s
    /// </summary>
    [DebuggerDisplay("Tag count: {Content.Count}")]
    public class PopulatedTagFeed : IFeed<TaggedGfyFeed>
    {
        readonly RequestOptions _defaultOptions;
        readonly GfycatClient _client;
        string IFeed<TaggedGfyFeed>.Cursor => _cursor;
        internal string _cursor { get; set; }
        readonly int _gfyCount;
        readonly int _tagCount;

        internal PopulatedTagFeed(GfycatClient client, int gfyCount, int tagCount, RequestOptions defaultOptions)
        {
            _defaultOptions = defaultOptions;
            _gfyCount = gfyCount;
            _tagCount = tagCount;
            _client = client;
        }
        /// <summary>
        /// Contains the current page of content for this feed
        /// </summary>
        public IReadOnlyCollection<TaggedGfyFeed> Content { get; private set; }
        
        internal static PopulatedTagFeed Create(GfycatClient client, int gfyCount, int tagCount, RequestOptions options, Model model)
        {
            return new PopulatedTagFeed(client, gfyCount, tagCount, options)
            {
                Content = model.Tags.Select(t => TaggedGfyFeed.Create(client, gfyCount, t, options)).ToReadOnlyCollection(),
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
        /// <summary>
        /// Returns the next page of this feed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IFeed<TaggedGfyFeed>> GetNextPageAsync(int count = GfycatClient.UseDefaultFeedCount, RequestOptions options = null)
        {
            if (count == GfycatClient.UseDefaultFeedCount)
                count = _tagCount;

            return Create(_client, _gfyCount, count, options, await _client.ApiClient.GetTrendingTagsPopulatedAsync(_cursor, count, _gfyCount, options).ConfigureAwait(false));
        }
    }
}
