using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class AddProviderParameters
    {
        [JsonProperty("provider")]
        internal string Provider { get; set; }
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
        [JsonProperty("secret")]
        internal string Secret { get; set; }
    }
}