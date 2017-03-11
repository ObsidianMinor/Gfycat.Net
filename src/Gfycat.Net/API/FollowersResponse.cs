using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API
{
    internal class FollowersResponse
    {
        [JsonProperty("followers")]
        internal IEnumerable<string> Followers { get; set; }
    }
}
