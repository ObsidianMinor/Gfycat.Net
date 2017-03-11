using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class CurrentUser : User
    {
        [JsonProperty("geoWhitelist")]
        internal IEnumerable<string> GeoWhitelist { get; set; }
        [JsonProperty("domainWhitelist")]
        internal IEnumerable<string> DomainWhitelist { get; set; }
        [JsonProperty("email")]
        internal string Email { get; set; }
        [JsonProperty("associatedProvders")]
        internal string AssociatedProviders { get; set; }
        [JsonProperty("uploadNotices")]
        internal bool UploadNotices { get; set; }
    }
}
