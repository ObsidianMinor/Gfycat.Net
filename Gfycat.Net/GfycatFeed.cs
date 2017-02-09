using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    public class GfycatFeed
    {
        [JsonProperty("gfycats")]
        IEnumerable<Gfy> Gfycats { get; set; }

        [JsonProperty("cursor")]
        string Cursor { get; set; }
    }
}
