using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class FacebookAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("auth_code")]
        internal string AuthCode { get; set; }

        [JsonProperty("token")]
        internal string Token { get; set; }
    }
}
