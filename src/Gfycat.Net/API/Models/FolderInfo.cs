using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class FolderInfo
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("nodes")]
        internal IEnumerable<FolderInfo> Subfolders { get; set; }
    }
}
