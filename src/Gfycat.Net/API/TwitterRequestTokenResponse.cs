using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class TwitterRequestTokenResponse
    {
        [JsonProperty("request_token")]
        internal string RequestToken { get; set; }
    }
}
