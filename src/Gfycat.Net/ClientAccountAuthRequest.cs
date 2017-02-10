using Newtonsoft.Json;

namespace Gfycat
{
    internal class ClientAccountAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("password")]
        internal string Password { get; set; }
    }
}
