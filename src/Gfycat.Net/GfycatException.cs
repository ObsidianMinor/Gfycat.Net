using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace Gfycat
{
    public class GfycatException : Exception
    {
        public HttpStatusCode HttpCode { get; set; }
        
        public string Code { get; set; }
        
        public string Description { get; set; }
        
        public GfycatException() : base()
        {

        }

        public GfycatException(string message, HttpStatusCode code) : this(code.ToString(), message, code)
        {
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
        
        internal static Exception CreateFromResponse(Rest.RestResponse restResponse)
        {
            string result = restResponse.ReadAsString();
            if (!string.IsNullOrWhiteSpace(result) && !result.StartsWith("<html>"))
            {
                JToken jsonObject = JToken.Parse(result);
                if (jsonObject.Type == JTokenType.Array)
                {
                    List<GfycatException> exceptions = new List<GfycatException>();
                    foreach (JToken token in jsonObject.Values<JToken>())
                    {
                        JToken gfyException = token["errorMessage"];
                        if (gfyException.Type == JTokenType.String)
                            exceptions.Add(new GfycatException(gfyException.Value<string>(), restResponse.Status));
                        else
                            exceptions.Add(new GfycatException(gfyException["code"].Value<string>(), gfyException["description"].Value<string>(), restResponse.Status));
                    }
                    return new AggregateException("Gfycat returned multiple errors", exceptions);
                }
                else
                {
                    JToken message = jsonObject["message"];
                    if (message == null)
                    {
                        JToken gfyException = jsonObject["errorMessage"];
                        if (gfyException.Type == JTokenType.String)
                            return new GfycatException(gfyException.Value<string>(), restResponse.Status);
                        else
                            return new GfycatException(gfyException["code"].Value<string>(), gfyException["description"].Value<string>(), restResponse.Status);
                    }
                    else
                        return new InvalidResourceException(message.Value<string>());
                }
            }
            else
            {
                return new GfycatException(restResponse.Status);
            }
        }
    }
}
