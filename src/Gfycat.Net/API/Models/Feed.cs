using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class Feed
    {
        [JsonProperty("gfycats")]
        public IEnumerable<Gfy> Gfycats { get; set; }
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }
}
