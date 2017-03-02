using Newtonsoft.Json;

namespace Gfycat.API.Requests
{
    internal class ClientCredentialsAuthRequest
    {   
        [JsonProperty("grant_type")]
        internal string GrantType { get; set; }

        [JsonProperty("client_id")]
        internal string ClientId { get; set; }

        [JsonProperty("client_secret")]
        internal string ClientSecret { get; set; }
    }
}
