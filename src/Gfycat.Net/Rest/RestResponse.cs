using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Gfycat.Rest
{
    public class RestResponse
    {
        public HttpStatusCode Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Stream Content { get; set; }
    }
}