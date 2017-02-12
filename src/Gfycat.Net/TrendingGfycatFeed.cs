using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gfycat
{
    public class TrendingGfycatFeed : GfycatFeed
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [JsonProperty("digest")]
        public string Digest { get; set; }
        [JsonProperty("newGfycats")]
        public List<Gfy> NewGfycats { get; set; }
    }
}