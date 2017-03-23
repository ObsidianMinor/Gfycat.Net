using System.Collections.Generic;
using System.Linq;

using Model = Gfycat.API.Models.TrendingTagsFeed;

namespace Gfycat
{
    public class PopulatedTagFeed : IFeed<TaggedGfyFeed>
    {
        readonly int _defaultGfyCount;
        readonly int _defaultTagCount;
        readonly RequestOptions _defaultOptions;
        readonly GfycatClient _client;
        
        internal PopulatedTagFeed(GfycatClient client, int defaultGfyCount, int defaultTagCount, RequestOptions defaultOptions)
        {
            _defaultGfyCount = defaultGfyCount;
            _defaultTagCount = defaultTagCount;
            _defaultOptions = defaultOptions;
            _client = client;
        }

        public IReadOnlyCollection<TaggedGfyFeed> Content { get; private set; }

        public string Cursor { get; private set; }

        internal static PopulatedTagFeed Create(GfycatClient client, int defaultGfyCount, int defaultTagCount, RequestOptions options, Model model)
        {
            return new PopulatedTagFeed(client, defaultGfyCount, defaultTagCount, options)
            {
                Content = model.Tags.Select(t => TaggedGfyFeed.Create(client, defaultGfyCount, t, options)).ToReadOnlyCollection(),
                Cursor = model.Cursor
            };
        }

        public IAsyncEnumerator<TaggedGfyFeed> GetEnumerator()
        {
            return new PopulatedTagFeedEnumerator(_client, this, _defaultTagCount, _defaultGfyCount, _defaultOptions);
        }
    }
}
