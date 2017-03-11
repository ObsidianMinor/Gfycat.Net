using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class ActionRequest
    {
        [JsonProperty("value")]
        internal object Value { get; set; }
        [JsonProperty("action")]
        internal string Action { get; set; }
    }
}
