using Newtonsoft.Json;

namespace Gfycat.API.Requests
{
    internal class ActionRequest
    {
        [JsonProperty("value")]
        internal string Value { get; set; }
        [JsonProperty("action")]
        internal string Action { get; set; }
    }
}
