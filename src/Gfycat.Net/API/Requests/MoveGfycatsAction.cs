using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    internal class GfyFolderAction
    {
        [JsonProperty("action")]
        internal string Action { get; set; }
        [JsonProperty("parent_id")]
        internal string ParentId { get; set; }
        [JsonProperty("gfy_ids")]
        internal IEnumerable<string> GfycatIds { get; set; }
    }
}
