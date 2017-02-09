using Newtonsoft.Json;

namespace Gfycat
{
    internal abstract class ProviderBaseAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("provider")]
        internal string Provider { get; set; }
    }
}
