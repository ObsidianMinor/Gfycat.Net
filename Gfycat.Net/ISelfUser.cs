using Newtonsoft.Json;

namespace Gfycat
{
    public interface ISelfUser : IUser
    {
        [JsonProperty("geoWhitelist")]
        string GeoWhitelist { get; }
        [JsonProperty("domainWhitelist")]
        string DomainWhitelist { get; }
        [JsonProperty("email")]
        string Email { get; }
    }
}
