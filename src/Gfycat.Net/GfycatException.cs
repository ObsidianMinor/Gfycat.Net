#pragma warning disable CS1591
using Gfycat.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;

namespace Gfycat
{
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }
        
        public string Code { get; set; }
        
        public string Description { get; set; }

        internal GfycatException() : base() { }

        internal GfycatException(string message) : base(message) { }

        internal GfycatException(string message, HttpStatusCode code) : this(code.ToString(), message, code) { }

        internal GfycatException(string code, string description, HttpStatusCode status) : base($"The server responded with {(int)status} {code}: {description}")
        {
            Code = code;
            Description = description;
            HttpCode = status;
        }

        internal GfycatException(HttpStatusCode status) : base($"The server responded with {(int)status}: {status}")
        {
            HttpCode = status;
        }
        
        internal static Exception CreateFromResponse(RestResponse restResponse, string readAsString = null)
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

            string result = readAsString ?? restResponse.ReadAsString();
            if (!string.IsNullOrWhiteSpace(result) && !result.StartsWith("<"))
            {
                JToken jsonObject = JToken.Parse(result);
                if (jsonObject.Type == JTokenType.Array)
                    return new AggregateException("Gfycat returned multiple errors", jsonObject.Values<JToken>().Select(token => ParseGfycatException(token["errorMessage"])));
                else
                {
                    JToken message = jsonObject["message"];
                    if (message == null)
                    {
                        JToken type = jsonObject["errorType"];
                        if (type == null)
                            return ParseGfycatException(jsonObject["errorMessage"]);
                        else
                            return new InternalServerException(jsonObject["errorMessage"].Value<string>(), type.Value<string>(), jsonObject["stackTrace"].Values<string>().ToArray(), restResponse.RequestMethod, restResponse.RequestUri);
                    }
                    else
                        return new InvalidResourceException($"Invalid resource: {restResponse.RequestMethod} method on endpoint \"{restResponse.RequestUri.AbsolutePath}\" does not exist!");
                }
            }
            else
                return new GfycatException(restResponse.Status);
        }

        internal static bool ContainsError(string responseString)
        {
            try
            {
                JToken token = JToken.Parse(responseString);
                return token["message"] != null || token["errorMessage"] != null;
            }
            catch(JsonReaderException)
            {
                return false;
            }
        }
    }
}
