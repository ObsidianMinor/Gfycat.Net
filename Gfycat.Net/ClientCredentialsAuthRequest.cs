using Newtonsoft.Json;

namespace Gfycat
{
    public class ClientCredentialsAuthRequest
    {   
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}
