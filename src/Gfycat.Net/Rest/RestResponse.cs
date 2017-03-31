using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Gfycat.Rest
{
    public class RestResponse
    {
        public HttpStatusCode Status { get; }
        public Dictionary<string, string> Headers { get; }
        public Stream Content { get; }
        public HttpMethod RequestMethod { get; }
        public Uri RequestUri { get; }

        public RestResponse(HttpStatusCode status, Dictionary<string, string> headers, Stream content, HttpMethod method, Uri requestUri)
        {
            Status = status;
            Headers = headers;
            Content = content;
            RequestMethod = method;
            RequestUri = requestUri;
        }
    }
}