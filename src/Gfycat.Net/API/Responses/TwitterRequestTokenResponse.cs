using Newtonsoft.Json;

namespace Gfycat.API.Responses
{
    internal class TwitterRequestTokenResponse
    {
        [JsonProperty("request_token")]
        internal string RequestToken { get; set; }
    }
}
