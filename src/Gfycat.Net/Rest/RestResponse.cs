using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Gfycat.Rest
{
    public class RestResponse
    {
        internal HttpStatusCode Status { get; set; }
        internal Dictionary<string, string> Headers { get; set; }
        internal Stream Content { get; set; }
    }
}