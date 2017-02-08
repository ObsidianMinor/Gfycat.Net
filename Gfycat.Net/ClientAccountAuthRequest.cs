using Newtonsoft.Json;

namespace Gfycat
{
    public class ClientAccountAuthRequest : ClientCredentialsAuthRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
