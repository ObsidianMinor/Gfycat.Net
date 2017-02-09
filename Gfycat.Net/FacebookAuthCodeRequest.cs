using Newtonsoft.Json;

namespace Gfycat
{
    internal class FacebookAuthCodeRequest : ProviderBaseAuthRequest
    {
        [JsonProperty("auth_code")]
        public string AuthCode { get; set; }
    }
}
