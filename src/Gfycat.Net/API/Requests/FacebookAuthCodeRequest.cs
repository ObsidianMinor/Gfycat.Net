using Newtonsoft.Json;

namespace Gfycat.API.Requests
{
    internal class FacebookAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("auth_code", NullValueHandling = NullValueHandling.Ignore)]
        internal string AuthCode { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        internal string Token { get; set; }
    }
}
