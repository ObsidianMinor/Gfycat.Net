using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatFolderInfo
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("nodes")]
        public IEnumerable<GfycatFolderInfo> Subfolders { get; private set; }
    }
}
