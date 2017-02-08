namespace Gfycat
{
    public class RefreshAuthRequest : ClientCredentialsAuthRequest
    {
        [Newtonsoft.Json.JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
