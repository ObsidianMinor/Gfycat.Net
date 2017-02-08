using Newtonsoft.Json;

namespace Gfycat
{
    public class ActionRequest
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}
