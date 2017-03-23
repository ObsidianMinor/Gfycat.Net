using Gfycat.API;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Gfycat
{
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }
        
        public string ServerMessage { get; set; }
        
        public string Code { get; set; }
        
        public string Description { get; set; }
        
        public GfycatException() : base()
        {

        }

        public GfycatException(string message) : base(message)
        {
            ServerMessage = message;
        }

        public GfycatException(string code, string description, HttpStatusCode status) : base($"The server responded with {(int)status} {code}: {description}")
        {
            Code = code;
            Description = description;
            HttpCode = status;
        }

        public GfycatException(HttpStatusCode status) : base($"The server responded with {(int)status}: {status}")
        {
            HttpCode = status;
        }
        
        internal static GfycatException CreateFromResponse(Rest.RestResponse restResponse)
        {
            string result = restResponse.ReadAsString();
            if (!string.IsNullOrWhiteSpace(result) && !result.StartsWith("<html>"))
            {
                ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(result);
                return new GfycatException(error.Error?.Code ?? restResponse.Status.ToString(), error.Error?.Description ?? error.Message, restResponse.Status);
            }
            else
            {
                return new GfycatException(restResponse.Status);
            }
        }
    }
}
