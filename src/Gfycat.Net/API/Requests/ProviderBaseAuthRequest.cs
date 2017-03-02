using Newtonsoft.Json;

namespace Gfycat.API.Requests
{
    internal abstract class ProviderBaseAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("provider")]
        internal string Provider { get; set; }
    }
}
