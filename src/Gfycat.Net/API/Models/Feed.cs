using Newtonsoft.Json;
using System.Collections.Generic;

#warning Does not expose number of found gfycats
namespace Gfycat.API.Models
{
    internal class Feed
    {
        [JsonProperty("gfycats")]
        public IEnumerable<Gfy> Gfycats { get; private set; }
        [JsonProperty("cursor")]
        public string Cursor { get; private set; }
    }
}
