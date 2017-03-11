using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API
{
    internal class FollowsResponse
    {
        [JsonProperty("follows")]
        internal IEnumerable<string> Follows { get; set; }
    }
}
