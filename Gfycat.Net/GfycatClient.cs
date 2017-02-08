using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : IDisposable
    {
        private WebWrapper _web = new WebWrapper();

        private string _clientId;
        private string _clientSecret;

        private string _scope;

        private string _accessToken;
        bool _tokenHasExpiration = true;

        private Timer _accessTokenExpiration;
        
        public bool AccessTokenExpired { get; private set; } = true;

        public ISelfUser CurrentUser => throw new NotImplementedException();

        public GfycatClient(string clientId, string clientSecret)
        {
            Debug.WriteLine($"Creating client with ID \"{clientId}\"");
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task AuthenticateAsync()
        {
            var auth = await _web.SendJsonAsync<ClientCredentialsAuthResponse>(
                HttpMethod.Post,
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "client_credentials"
                });
            _scope = auth.Scope;
            AccessTokenExpired = false;
            _accessToken = auth.AccessToken;
            _accessTokenExpiration = new Timer(AccessTokenExpirationCallback, null, auth.ExpiresIn, Timeout.Infinite);
        }

        public async Task AuthenticateClientCreatorAsync(string username, string password)
        {
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                HttpMethod.Put,
                "oauth/token",
                new ClientAccountAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "password",
                    Username = username,
                    Password = password
                });
            _scope = auth.Scope;

            _accessTokenExpiration?.Dispose();
            AccessTokenExpired = false;
            _accessToken = auth.AccessToken;
            _accessTokenExpiration = new Timer(AccessTokenExpirationCallback, null, auth.ExpiresIn, Timeout.Infinite);
        }

        public void Authenticate(string accessToken, int accessTokenExpiresIn, string scope)
        {
            _scope = scope;

            _accessTokenExpiration?.Dispose();
            AccessTokenExpired = false;
            _accessToken = accessToken;
            _accessTokenExpiration = new Timer(AccessTokenExpirationCallback, null, accessTokenExpiresIn, Timeout.Infinite);
        }

        public string GenerateBrowserAuthUrl(string state, string redirectUri) => $"https://gfycat.com/oauth/authorize?client_id={_clientId}&scope=all&state={state}&response_type=token&redirect_uri={redirectUri}";

        public Task<GfycatStatus> CheckGfycatUploadStatus(string gfycat)
        {
            return _web.SendRequestAsync<GfycatStatus>(HttpMethod.Get, $"gfycats/fetch/status/{gfycat}");
        }

        public Task CreateAccount()
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateGfycat(Stream file, GfycatCreationParameters parameters)
        {
            GfycatKey uploadKey = await _web.SendJsonAsync<GfycatKey>(HttpMethod.Post, "gfycats", parameters, _accessToken);
            throw new NotImplementedException();
        }

        public async Task<string> CreateGfycat(string remoteUrl, GfycatCreationParameters parameters)
        {
            throw new NotImplementedException();
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

        public Task<bool> SendPasswordResetEmailAsync(string usernameOrEmail)
        {
            throw new NotImplementedException();
        }

        public Task ShareGfycatOnTwitter(string gfycatId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UsernameIsValidAsync(string username)
        {
            throw new NotImplementedException();
        }

        private void AccessTokenExpirationCallback(object state)
        {
            Debug.WriteLine("Access token expired.");
            AccessTokenExpired = true;
        }
    }
}
