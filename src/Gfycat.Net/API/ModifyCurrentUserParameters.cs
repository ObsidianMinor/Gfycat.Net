using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API
{
    internal class ModifyCurrentUserParameters
    {
        [JsonProperty("operations")]
        internal IEnumerable<GfycatOperation> Operations { get; set; }
    }
}