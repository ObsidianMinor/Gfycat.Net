using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class TwitterShareRequest
    {
        [JsonProperty("status")]
        internal string Status { get; set; }
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
    }
}
