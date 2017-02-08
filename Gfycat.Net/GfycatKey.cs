using Newtonsoft.Json;

namespace Gfycat
{
    internal class GfycatKey
    {
        [JsonProperty("isOk")]
        public bool IsOK { get; set; }
        [JsonProperty("gfycat")]
        public string Gfycat { get; set; }
        [JsonProperty("secret")]
        public string Secret { get; set; }
        [JsonProperty("uploadType")]
        public string UploadType { get; set; }
    }
}
