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
        
        public override string Message { get => !string.IsNullOrWhiteSpace(base.Message) ? $"The server responded with \"{base.Message}\"" : $"The server responded with \"{this}\""; }

        public override string ToString()
        {
            return $"{(int)HttpCode} {Code}: {Description}";
        }

        public GfycatException() : base() { }

        [JsonConstructor]
        public GfycatException([JsonProperty("errorMessage")] string message) : base(message) { }
    }
}
