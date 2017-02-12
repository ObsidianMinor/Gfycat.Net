using Newtonsoft.Json;

namespace Gfycat
{
    public class CurrentUser : User
    {
        [JsonProperty("geoWhitelist")]
        string GeoWhitelist { get; set; }
        [JsonProperty("domainWhitelist")]
        string DomainWhitelist { get; set; }
        [JsonProperty("email")]
        string Email { get; set; }
    }
}
