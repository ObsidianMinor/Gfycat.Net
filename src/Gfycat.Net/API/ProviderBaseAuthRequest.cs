using Newtonsoft.Json;

namespace Gfycat.API
{
    internal abstract class ProviderBaseAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("provider")]
        internal string Provider { get; set; }
    }
}
