using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class TwitterAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
        [JsonProperty("tokenSecret")]
        internal string Secret { get; set; }
    }
}
