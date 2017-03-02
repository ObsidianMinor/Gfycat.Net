namespace Gfycat.API.Requests
{
    internal class RefreshAuthRequest : ClientCredentialsAuthRequest
    {
        [Newtonsoft.Json.JsonProperty("refresh_token")]
        internal string RefreshToken { get; set; }
    }
}
