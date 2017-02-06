using Newtonsoft.Json;

namespace Gfycat
{
    /// <summary>
    /// Represents an authentication response using client credentials
    /// </summary>
    public class SimpleClientAuthResponse
    {
        [JsonProperty("token_type")]
        string TokenType { get; set; }

        [JsonProperty("scope")]
        string Scope { get; set; }

        [JsonProperty("expires_in")]
        int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        string AccessToken { get; set; }
    }
}
