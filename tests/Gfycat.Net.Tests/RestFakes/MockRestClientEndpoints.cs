using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gfycat.Net.Tests.RestFakes
{
    internal partial class MockRestClient
    {
        private Task<HttpResponseMessage> MakeResponse(HttpStatusCode code, HttpContent content)
            => Task.FromResult(new HttpResponseMessage(code)
            {
                Content = content
            });

        private Task<HttpResponseMessage> MakeResponse(HttpStatusCode code, string content = "") => MakeResponse(code, new StringContent(content));

        bool _followingValid = false;

        [RestEndpoint("HEAD", "users/" + Resources.ValidUserName)]
        public Task<HttpResponseMessage> GetValidUserExists(HttpRequestMessage message) => MakeResponse(HttpStatusCode.OK);

        [RestEndpoint("HEAD", "users/" + Resources.InvalidUserName)]
        public Task<HttpResponseMessage> GetInvalidUserExists(HttpRequestMessage message) => MakeResponse(HttpStatusCode.NotFound);

        [RestEndpoint("GET", "users/" + Resources.ValidUserName)]
        public Task<HttpResponseMessage> GetUser(HttpRequestMessage message) => MakeResponse(HttpStatusCode.OK, Resources.ValidUserInfo);

        [RestEndpoint("GET", "users/" + Resources.InvalidUserName)]
        public Task<HttpResponseMessage> GetInvalidUser(HttpRequestMessage message) => MakeResponse(HttpStatusCode.NotFound);

        [RestEndpoint("GET", "me")]
        public Task<HttpResponseMessage> GetMe(HttpRequestMessage message) => throw new NotImplementedException();

        [RestEndpoint("HEAD", "me/follows/" + Resources.ValidUserName)]
        public Task<HttpResponseMessage> FollowingValid(HttpRequestMessage message) => MakeResponse(_followingValid ? HttpStatusCode.OK : HttpStatusCode.NotFound);

        [RestEndpoint("PUT", "me/follows/" + Resources.ValidUserName)]
        public Task<HttpResponseMessage> FollowValid(HttpRequestMessage message)
        {
            _followingValid = true;
            return MakeResponse(HttpStatusCode.OK);
        }

        [RestEndpoint("DELETE", "me/follows/" + Resources.ValidUserName)]
        public Task<HttpResponseMessage> UnfollowValid(HttpRequestMessage message)
        {
            _followingValid = false;
            return MakeResponse(HttpStatusCode.OK);
        }

        [RestEndpoint("POST", "oauth/token")]
        public async Task<HttpResponseMessage> AuthenticateAsync(HttpRequestMessage message)
        {
            switch(await message.Content.ReadAsStringAsync())
            {
                case Resources.ClientCredentialsRequest:
                    return await MakeResponse(HttpStatusCode.OK, Resources.ClientCredentialsResponse);
                default:
                    return await MakeResponse(HttpStatusCode.OK, "");
            }
        }
    }
}
