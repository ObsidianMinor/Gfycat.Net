using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API
{
    internal class UserAlbumsResponse
    {
        [JsonProperty("totalItemCount")]
        internal int TotalItemCount { get; set; }
        [JsonProperty("items")]
        internal IEnumerable<Models.AlbumInfo> Items { get; set; }
    }
}
