using Newtonsoft.Json;

namespace Gfycat.OAuth2
{
    internal class TwitterAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
    }
}
