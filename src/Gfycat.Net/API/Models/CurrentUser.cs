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
        [JsonProperty("emailVerified")]
        internal bool EmailVerified { get; set; }
        [JsonProperty("uploadNotices")]
        internal bool UploadNotices { get; set; }
        [JsonProperty("totalGfycats")]
        internal int TotalGfycats { get; set; }
        [JsonProperty("totalAlbums")]
        internal int TotalAlbums { get; set; }
        [JsonProperty("totalBookmarks")]
        internal int TotalBookmarks { get; set; }
    }
}
