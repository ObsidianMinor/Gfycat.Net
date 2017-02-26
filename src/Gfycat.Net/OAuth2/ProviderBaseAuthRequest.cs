using Newtonsoft.Json;

namespace Gfycat.OAuth2
{
    internal abstract class ProviderBaseAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("provider")]
        internal string Provider { get; set; }
    }
}
