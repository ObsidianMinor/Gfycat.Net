namespace Gfycat.API
{
    internal class RefreshAuthRequest : ClientCredentialsAuthRequest
    {
        [Newtonsoft.Json.JsonProperty("refresh_token")]
        internal string RefreshToken { get; set; }
    }
}
