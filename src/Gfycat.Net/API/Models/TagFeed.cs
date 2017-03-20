using Newtonsoft.Json;

namespace Gfycat.API.Models
{
    internal class TagFeed : Feed
    {
        [JsonProperty("tag")]
        internal string Tag { get; set; }
    }
}
