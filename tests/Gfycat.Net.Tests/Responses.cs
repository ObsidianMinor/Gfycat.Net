using Gfycat.Rest;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gfycat.Net.Tests
{
    internal static class Responses
    {
        static Dictionary<string, Dictionary<string, RestResponse>> NoJsonInputResponses = new Dictionary<string, Dictionary<string, RestResponse>>
        {
            {
                "GET",
                new Dictionary<string, RestResponse>()
                {
                    {
                        "users/validUser",
                        new RestResponse()
                        {
                            Status = System.Net.HttpStatusCode.OK,
                            Content = StreamFromString("{\"userid\":\"validUser\",\"description\":\"Testing Gfycat.Net\",\"profileUrl\":\"https:\\/\\/github.com\\/ObsidianMinor\\/Gfycat.Net\",\"profileImageUrl\":\"https:\\/\\/profiles.gfycat.com\\/9c907b40aa37fbc7e8655bfe80a75d11eb0286e78f5a3cababf93c32a89b723b.png\",\"name\":\"validUser\",\"username\":\"validUser\",\"views\":\"546\",\"verified\":false,\"iframeProfileImageVisible\":false,\"followers\":\"1\",\"following\":\"0\",\"publishedGfycats\":\"15\",\"publishedAlbums\":\"1\",\"createDate\":\"1486347089\",\"url\":\"https:\\/\\/gfycat.com\\/@obsidianminor\"}"),
                        }
                    },
                    {
                        "users/invalidUser",
                        new RestResponse()
                        {
                            Status = System.Net.HttpStatusCode.NotFound,
                            Content = StreamFromString("")
                        }
                    }
                }
            },
            {
                "HEAD",
                new Dictionary<string, RestResponse>()
                {
                    {
                        "me/follows/validUser",
                        new RestResponse()
                        {
                            Status = System.Net.HttpStatusCode.NotFound,
                            Content = StreamFromString("")
                        }
                    }
                }
            },
            {
                "PUT",
                new Dictionary<string, RestResponse>()
                {

                }
            },
            {
                "DELETE",
                new Dictionary<string, RestResponse>()
                {

                }
            },
        };

        internal static RestResponse GetResponse(string method, string endpoint)
        {
            return NoJsonInputResponses[method][endpoint];
        }

        internal static RestResponse GetResponse(string method, string endpoint, string json)
        {
            throw new NotImplementedException();
        }
        
        internal static RestResponse GetResponse(string method, string endpoint, Stream stream)
        {
            throw new NotImplementedException();
        }

        internal static RestResponse GetResponse(string method, string endpoint, IDictionary<string, object> multipart)
        {
            throw new NotImplementedException();
        }

        internal static Stream StreamFromString(string input)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
