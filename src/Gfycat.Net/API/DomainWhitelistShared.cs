using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API
{
    internal class DomainWhitelistShared
    {
        [JsonProperty("domainWhitelist")]
        internal IEnumerable<string> DomainWhitelist { get; set; }
    }
}
