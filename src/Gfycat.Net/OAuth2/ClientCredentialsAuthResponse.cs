using Newtonsoft.Json;

namespace Gfycat.OAuth2
{
    /// <summary>
    /// Represents an authentication response using client credentials
    /// </summary>
    internal class ClientCredentialsAuthResponse
    {
        [JsonProperty("token_type")]
        internal string TokenType { get; set; }

        [JsonProperty("scope")]
        internal string Scope { get; set; }

        [JsonProperty("expires_in")]
        internal int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        internal string AccessToken { get; set; }
    }
}
