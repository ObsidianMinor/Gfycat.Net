using Newtonsoft.Json;

namespace Gfycat
{
    internal class ActionRequest
    {
        [JsonProperty("value")]
        internal string Value { get; set; }
        [JsonProperty("action")]
        internal string Action { get; set; }
    }
}
