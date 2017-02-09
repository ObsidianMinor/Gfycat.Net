using Newtonsoft.Json;

namespace Gfycat
{
    internal class TwitterAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
    }
}
