using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat.API
{
    internal class GeoWhitelistShared
    {
        [JsonProperty("geoWhitelist")]
        internal IEnumerable<string> GeoWhitelist { get; set; }
    }
}
