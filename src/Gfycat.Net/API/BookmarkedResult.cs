using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class BookmarkedResult
    {
        [JsonProperty("bookmarkState", ItemConverterType = typeof(Converters.NumericalBooleanConverter))]
        internal bool BookmarkStatus { get; set; }
    }
}
