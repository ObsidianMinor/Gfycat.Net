using Newtonsoft.Json;

namespace Gfycat
{
    /// <summary>
    /// Represents a response using a password grant authentication
    /// </summary>
    public class ClientAccountAuthResponse : ClientCredentialsAuthResponse
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
        [JsonProperty("resource_owner")]
        public string ResourceOwner { get; set; }
    }
}
