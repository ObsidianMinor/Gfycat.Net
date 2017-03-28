using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class AccountCreationRequest
    {
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("password")]
        internal string Password { get; set; }
        [JsonProperty("email")]
        internal string Email { get; set; }
        [JsonProperty("provider")]
        internal string Provider { get; set; }
        [JsonProperty("auth_code")]
        internal string AuthCode { get; set; }
        [JsonProperty("access_token")]
        internal string AccessToken { get; set; }
        [JsonProperty("token")]
        internal string Token { get; set; }
        [JsonProperty("verifier")]
        internal string Verifier { get; set; }
        [JsonProperty("oauth_token")]
        internal string OauthToken { get; set; }
        [JsonProperty("oauth_token_secret")]
        internal string OauthTokenSecret { get; set; }
        [JsonProperty("secret")]
        internal string Secret { get; set; }
    }
}
