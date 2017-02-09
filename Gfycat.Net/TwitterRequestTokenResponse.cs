using Newtonsoft.Json;

namespace Gfycat
{
    internal class TwitterRequestTokenResponse
    {
        [JsonProperty("request_token")]
        internal string RequestToken { get; set; }
    }
}
