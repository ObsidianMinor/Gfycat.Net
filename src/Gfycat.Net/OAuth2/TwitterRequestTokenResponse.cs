using Newtonsoft.Json;

namespace Gfycat.OAuth2
{
    internal class TwitterRequestTokenResponse
    {
        [JsonProperty("request_token")]
        internal string RequestToken { get; set; }
    }
}
