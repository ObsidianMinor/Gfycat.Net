using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    public class GfycatAlbumResponse
    {
        [JsonProperty("totalItemCount")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public IEnumerable<GfycatAlbumInfo> Albums { get; set; }
    }
}
