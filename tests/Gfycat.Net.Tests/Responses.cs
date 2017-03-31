using Gfycat.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Gfycat.Net.Tests
{
    internal class Responses
    {
        static readonly Stream Empty = StreamFromString("");
        static readonly RestResponse OK = new RestResponse(HttpStatusCode.OK, null, Empty, null, null);
        static readonly RestResponse NotFound = new RestResponse(HttpStatusCode.NotFound, null, Empty, null, null);
        Dictionary<string, Dictionary<string, RestResponse>> NoJsonInputResponses = new Dictionary<string, Dictionary<string, RestResponse>>
        {
            {
                "GET",
                new Dictionary<string, RestResponse>()
                {
                    {
                        "users/validUser",
                        new RestResponse(
                            HttpStatusCode.OK,
                            null,
                            StreamFromString("{\"userid\":\"validUser\",\"description\":\"Testing Gfycat.Net\",\"profileUrl\":\"https:\\/\\/github.com\\/ObsidianMinor\\/Gfycat.Net\",\"profileImageUrl\":\"https:\\/\\/profiles.gfycat.com\\/9c907b40aa37fbc7e8655bfe80a75d11eb0286e78f5a3cababf93c32a89b723b.png\",\"name\":\"validUser\",\"username\":\"validUser\",\"views\":\"546\",\"verified\":false,\"iframeProfileImageVisible\":false,\"followers\":\"1\",\"following\":\"0\",\"publishedGfycats\":\"15\",\"publishedAlbums\":\"1\",\"createDate\":\"1486347089\",\"url\":\"https:\\/\\/gfycat.com\\/@obsidianminor\"}"),
                            null,
                            null)
                    },
                    { "users/invalidUser", NotFound }
                }
            },
            {
                "HEAD",
                new Dictionary<string, RestResponse>()
                {
                    { "me/follows/validUser", NotFound },
                    { "users/" + MockRestClient.ValidUser, OK }
                }
            },
            {
                "PUT",
                new Dictionary<string, RestResponse>()
                {
                    { "me/follows/validUser", OK }
                }
            },
            {
                "DELETE",
                new Dictionary<string, RestResponse>()
                {
                    { "me/follows/validUser", OK }
                }
            },
        };

        internal RestResponse GetResponse(string method, string endpoint)
        {
            return NoJsonInputResponses[method][endpoint];
        }

        internal RestResponse GetResponse(string method, string endpoint, string json)
        {
            throw new NotImplementedException();
        }
        
        internal RestResponse GetResponse(string method, string endpoint, Stream stream)
        {
            throw new NotImplementedException();
        }

        internal RestResponse GetResponse(string method, string endpoint, IDictionary<string, object> multipart)
        {
            throw new NotImplementedException();
        }

        internal static Stream StreamFromString(string input)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
