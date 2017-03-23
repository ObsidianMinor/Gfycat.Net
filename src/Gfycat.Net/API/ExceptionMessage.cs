using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat.API
{
    internal class ExceptionMessage
    {
        [JsonProperty("code")]
        internal string Code { get; set; }
        [JsonProperty("description")]
        internal string Description { get; set; }
    }
}
