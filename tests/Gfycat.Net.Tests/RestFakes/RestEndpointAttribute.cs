using System;

namespace Gfycat.Net.Tests.RestFakes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestEndpointAttribute : Attribute
    {
        public string Method { get; }

        public string Endpoint { get; }

        public RestEndpointAttribute(string method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
        }
    }
}
