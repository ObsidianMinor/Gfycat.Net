using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Gfycat
{
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }
        
        public string Code { get; set; }
        
        public string Description { get; set; }

        public GfycatException() : base() { }

        public GfycatException(string message) : base(message) { }

        public GfycatException(string message, HttpStatusCode code) : this(code.ToString(), message, code) { }
        
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
        
        internal static Exception CreateFromResponse(Rest.RestResponse restResponse)
        {
            GfycatException ParseGfycatException(JToken gfyException)
            {
                if (gfyException.Type == JTokenType.String)
                {
                    try
                    {
                        JToken internalMessage = JToken.Parse(gfyException.Value<string>());
                        return new GfycatException(internalMessage.Value<string>("code"), internalMessage.Value<string>("description"), restResponse.Status);
                    }
                    catch (JsonReaderException)
                    {
                        return new GfycatException(gfyException.Value<string>(), restResponse.Status);
                    }
                }
                else
                    return new GfycatException(gfyException.Value<string>("code"), gfyException.Value<string>("description"), restResponse.Status);
            }

            string result = restResponse.ReadAsString();
            if (!string.IsNullOrWhiteSpace(result) && !result.StartsWith("<"))
            {
                JToken jsonObject = JToken.Parse(result);
                if (jsonObject.Type == JTokenType.Array)
                    return new AggregateException("Gfycat returned multiple errors", jsonObject.Values<JToken>().Select(token => ParseGfycatException(token["errorMessage"])));
                else
                {
                    JToken message = jsonObject["message"];
                    if (message == null)
                        return ParseGfycatException(jsonObject["errorMessage"]);
                    else
                        return new InvalidResourceException(message.Value<string>());
                }
            }
            else
                return new GfycatException(restResponse.Status);
        }
    }
}
