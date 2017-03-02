using Newtonsoft.Json;

namespace Gfycat.API.Requests
{
    internal class TwitterAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
    }
}
