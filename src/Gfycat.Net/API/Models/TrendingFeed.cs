using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class TrendingFeed : Feed
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [JsonProperty("digest")]
        public string Digest { get; set; }
        [JsonProperty("newGfycats")]
        public List<Gfy> NewGfycats { get; set; }
    }
}
