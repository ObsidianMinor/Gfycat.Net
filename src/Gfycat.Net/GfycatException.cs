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
        
        public override string Message
        {
            get
            {
                if (Description == null)
                    return $"The server responded with {this}";
                else if(!_baseMessageNull)
                    return $"The server responded with \"{base.Message}\"";
                else
                    return $"The server responded with {(int)HttpCode}: {HttpCode}";
            }
        }

        private readonly bool _baseMessageNull;

        public override string ToString()
        {
            return $"{(int)HttpCode} {Code}: \"{Description}\"";
        }

        public GfycatException() : base() { }

        [JsonConstructor]
        public GfycatException([JsonProperty("message")] string message) : base(message)
        {
            _baseMessageNull = message == null;
        }
    }
}
