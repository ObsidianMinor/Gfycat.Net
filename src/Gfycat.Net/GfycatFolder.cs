using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatFolder
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("nodes")]
        public List<GfycatFolder> Subfolders { get; set; }
    }
}
