using Newtonsoft.Json;

namespace Gfycat.API.Models
{
    internal class SearchFeed : Feed
    {
        [JsonProperty("found")]
        internal int Found { get; set; }
    }
}
