using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Gfycat.Rest
{
    /// <summary>
    /// Represents a response from a REST endpoint
    /// </summary>
    public class RestResponse
    {
        /// <summary>
        /// The status returned from the endpoint
        /// </summary>
        public HttpStatusCode Status { get; }
        /// <summary>
        /// The headers returned from the endpoint
        /// </summary>
        public Dictionary<string, string> Headers { get; }
        /// <summary>
        /// The content returned from the endpoint
        /// </summary>
        public Stream Content { get; }
        /// <summary>
        /// The method used in the request to the endpoint
        /// </summary>
        public HttpMethod RequestMethod { get; }
        /// <summary>
        /// The URI endpoint used in the request
        /// </summary>
        public Uri RequestUri { get; }

        /// <summary>
        /// Creates a new <see cref="RestResponse"/> with the specified status, headers, content, method, and request URI
        /// </summary>
        /// <param name="status"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <param name="requestUri"></param>
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