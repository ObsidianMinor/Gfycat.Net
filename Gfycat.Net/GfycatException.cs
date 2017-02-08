using Newtonsoft.Json;
using System;
using System.Net;

namespace Gfycat
{
    [JsonObject("errorMessage")]
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
