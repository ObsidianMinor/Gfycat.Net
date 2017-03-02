using Gfycat.API.Responses;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Gfycat
{
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }

        [JsonProperty("message")]
        public string ServerMessage { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        public override string Message
        {
            get
            {
                if (ServerMessage != null)
                    return $"The server responded with {(int)HttpCode}: {ServerMessage}";
                else if (Description != null)
                    return $"The server responded with {(int)HttpCode} {Code}: {Description}";
                else
                    return $"The server responded with {(int)HttpCode}: {HttpCode}";
            }
        }

        public GfycatException() : base() { }

        internal static GfycatException CreateFromResponse(Rest.RestResponse restResponse)
        {
            ErrorResponse response = new ErrorResponse()
            {
                Error = new GfycatException()
                {
                    HttpCode = restResponse.Status
                }
            };
            string result = restResponse.ReadAsString();
            if (!string.IsNullOrWhiteSpace(result) && !result.StartsWith("<html>"))
                JsonConvert.PopulateObject(result, response);
            return response.Error;
        }
    }
}
