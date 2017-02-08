using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : IDisposable
    {
        private WebWrapper _web;

        private string _clientId;
        private string _clientSecret;

        private string _accessToken;

        private string _refreshToken;

        public ISelfUser CurrentUser { get; set; }

        public GfycatClient(string clientId, string clientSecret)
        {
            _web = new WebWrapper();
            Debug.WriteLine($"Creating client with ID \"{clientId}\"");
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        #region Authentication

        public async Task RefreshToken()
        {
            ClientAccountAuthResponse response = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new RefreshAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "refresh",
                    RefreshToken = _refreshToken
                });

            _refreshToken = response.RefreshToken;
            _accessToken = response.AccessToken;
        }

        public async Task AuthenticateAsync()
        {
            var auth = await _web.SendJsonAsync<ClientCredentialsAuthResponse>(
                "POST",
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "client_credentials"
                });
            
            _accessToken = auth.AccessToken;
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateAsync(string username, string password)
        {
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new ClientAccountAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "password",
                    Username = username,
                    Password = password
                });
            
            _accessToken = auth.AccessToken;
            _refreshToken = auth.RefreshToken;
            await UpdateCurrentUser();
        }

        public async void Authenticate(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            await UpdateCurrentUser();
        }

        public async Task UpdateCurrentUser()
        {
            CurrentUser = await _web.SendRequestAsync<ISelfUser>("GET", "me", _accessToken);
        }

        public string GetBrowserAuthUrl(string state, string redirectUri) => $"https://gfycat.com/oauth/authorize?client_id={_clientId}&scope=all&state={state}&response_type=token&redirect_uri={redirectUri}";

        #endregion

        public Task<GfycatStatus> CheckGfycatUploadStatus(string gfycat)
        {
            return _web.SendRequestAsync<GfycatStatus>("GET", $"gfycats/fetch/status/{gfycat}");
        }

        public Task CreateAccount()
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateGfycat(Stream file, GfycatCreationParameters parameters)
        {
            GfycatKey uploadKey = await _web.SendJsonAsync<GfycatKey>("POST", "gfycats", parameters, _accessToken);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Gfycat using a remote url and returns the Gfy name
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> CreateGfycat(string remoteUrl, GfycatCreationParameters parameters)
        {
            parameters.FetchUrl = remoteUrl;
            return (await _web.SendJsonAsync<GfycatKey>("POST", "gfycats", parameters, _accessToken)).Gfycat;
        }

        public void Dispose()
        {
            ((IDisposable)_web).Dispose();
        }

        public Task<IEnumerable<IGfy>> GetUserGfycatFeed(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGfy>> GetUserGfycatFeed(string userId, int count)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGfy>> GetUserGfycatFeed(string userId, int count, string cursor)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetEmailAsync(string usernameOrEmail)
        {
            return _web.SendJsonAsync("PATCH", "users", new ActionRequest() { Value = usernameOrEmail, Action = "send_password_reset_email" }, _accessToken);
        }

        public Task ShareGfycatOnTwitter(string gfycatId, string postStatus)
        {
            return _web.SendJsonAsync("POST", $"gfycats/{gfycatId}/share/twitter", new { status = postStatus }, _accessToken);
        }

        public async Task<bool> UsernameIsValidAsync(string username)
        {
            var statusCode = await _web.SendRequestForStatusAsync("HEAD", $"users/{username}", _accessToken, true);
            return statusCode == HttpStatusCode.NotFound;
        }
    }
}
