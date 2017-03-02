using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Responses
{
    internal class GfycatAlbumResponse
    {
        [JsonProperty("totalItemCount")]
        internal int Count { get; set; }
        [JsonProperty("items")]
        internal IEnumerable<GfycatAlbumInfo> Albums { get; set; }
    }
}
