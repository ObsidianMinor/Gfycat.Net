using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gfycat.API.Models
{
    internal class Status
    {
        [JsonProperty("task"), JsonConverter(typeof(StringEnumConverter))]
        public UploadTask Task { get; set; }
        [JsonProperty("time")]
        public int Time { get; set; }
        [JsonProperty("gfyname")]
        public string GfyName { get; set; }
        [JsonProperty("description")]
        public string ErrorDescription { get; set; }
    }
}
