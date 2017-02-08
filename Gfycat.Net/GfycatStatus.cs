using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gfycat
{
    public class GfycatStatus
    {
        [JsonProperty("task"), JsonConverter(typeof(StringEnumConverter))]
        public Status Task { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("gfyname")]
        public string GfyName { get; set; }
    }
}
