using Newtonsoft.Json;

namespace Gfycat
{
    /// <summary>
    /// Represents a response using a password grant authentication
    /// </summary>
    internal class ClientAccountAuthResponse : ClientCredentialsAuthResponse
    {
        [JsonProperty("refresh_token")]
        internal string RefreshToken { get; set; }
        [JsonProperty("refresh_token_expires_in")]
        internal int RefreshTokenExpiresIn { get; set; }
        [JsonProperty("resource_owner")]
        internal string ResourceOwner { get; set; }
    }
}
