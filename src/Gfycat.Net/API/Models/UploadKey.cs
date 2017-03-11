using Newtonsoft.Json;

namespace Gfycat.API.Models
{
    internal class UploadKey
    {
        [JsonProperty("isOk")]
        internal bool IsOK { get; set; }
        [JsonProperty("gfyname")]
        internal string Gfycat { get; set; }
        [JsonProperty("secret")]
        internal string Secret { get; set; }
        [JsonProperty("uploadType")]
        internal string UploadType { get; set; }
        [JsonProperty("fetchUrl")]
        internal string FetchUrl { get; set; }
    }
}
