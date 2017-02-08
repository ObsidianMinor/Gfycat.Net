using Newtonsoft.Json;
using System;

namespace Gfycat
{
    [JsonObject("errorMessage")]
    public class GfycatException : Exception
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
