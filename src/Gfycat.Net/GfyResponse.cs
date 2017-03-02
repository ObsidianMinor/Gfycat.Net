using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat
{
    internal class GfyResponse
    {
        [JsonProperty("gfyItem")]
        internal Gfy GfyItem { get; set; }
    }
}
