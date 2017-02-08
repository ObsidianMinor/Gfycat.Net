using Newtonsoft.Json;

namespace Gfycat
{
    /// <summary>
    /// Represents an authentication response using client credentials
    /// </summary>
    public class ClientCredentialsAuthResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
