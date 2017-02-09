using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : IDisposable
    {
        const string _startEndpoint = "https://api.gfycat.com/v1/";
        private HttpClient _web;

        private string _clientId;
        private string _clientSecret;

        private string _accessToken;

        private string _refreshToken;
        
        public GfycatClient(string clientId, string clientSecret)
        {
            _web = new HttpClient() { BaseAddress = new Uri(_startEndpoint) };

            Debug.WriteLine($"Creating client with ID \"{clientId}\"");
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        #region Authentication

        public async Task RefreshTokenAsync()
        {
            Debug.WriteLine("Refreshing token...");
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new RefreshAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "refresh",
                    RefreshToken = _refreshToken
                });

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            _refreshToken = auth.RefreshToken;
            _accessToken = auth.AccessToken;
            await FinishAuth();
        }

        public async Task AuthenticateAsync()
        {
            Debug.WriteLine("Performing client credentials authentication...");
            var auth = await _web.SendJsonAsync<ClientCredentialsAuthResponse>(
                "POST",
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "client_credentials"
                });

            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            _accessToken = auth.AccessToken;
            await FinishAuth();
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateAsync(string username, string password)
        {
            Debug.WriteLine($"Performing client account authentication...");
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
            Debug.WriteLine($"Logged in as {username}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            _accessToken = auth.AccessToken;
            _refreshToken = auth.RefreshToken;
            await FinishAuth();
        }

        public async Task AuthenticateFacebookCodeAsync(string authCode)
        {
            Debug.WriteLine($"Performing account authentication using Facebook...");
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST", 
                "oauth/token", 
                new FacebookAuthCodeRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "convert_code",
                    Provider = "facebook",
                    AuthCode = authCode
                });

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            _accessToken = auth.AccessToken;
            _refreshToken = auth.RefreshToken;
            await FinishAuth();
        }

        public async Task AuthenticateFacebookTokenAsync(string token)
        {
            Debug.WriteLine("Performing account authentication using Facebook...");
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new FacebookAuthCodeRequest()
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "convert_code",
                    Provider = "facebook",
                    Token = token
                });

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            _accessToken = auth.AccessToken;
            _refreshToken = auth.RefreshToken;
            await FinishAuth();
        }

        public async Task<string> GetTwitterRequestTokenAsync()
        {
            Debug.WriteLine("Getting Twitter request token...");
            var token = await _web.SendJsonAsync<TwitterRequestTokenResponse>(
                "POST", 
                "oauth/token", 
                new
                {
                    grant_type = "request_token",
                    provider = "twitter"
                });
            return token.RequestToken;
        }

        public async Task AuthenticateTwitterTokenAsync(string requestToken, string verifier)
        {
            Debug.WriteLine("Performing account authentication using Twitter...");
            var auth = await _web.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new TwitterAuthCodeRequest()
                {
                    GrantType = "convert_request_token",
                    Provider = "twitter",
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    Token = requestToken,
                    Verifier = verifier
                });

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            _accessToken = auth.AccessToken;
            _refreshToken = auth.RefreshToken;
            await FinishAuth();
        }

        public async void Authenticate(string accessToken, string refreshToken)
        {
            Debug.WriteLine($"Recieved access token {accessToken}");
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            await FinishAuth();
        }

        public string GetBrowserAuthUrl(string state, string redirectUri) => $"https://gfycat.com/oauth/authorize?client_id={_clientId}&scope=all&state={state}&response_type=token&redirect_uri={redirectUri}";
        
        internal async Task CheckAuthorization(string endpoint)
        {
            if (await _web.SendRequestForStatusAsync("HEAD", endpoint, _accessToken) == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();
                if (await _web.SendRequestForStatusAsync("HEAD", endpoint, _accessToken) == HttpStatusCode.Unauthorized)
                    throw new GfycatException()
                    {
                        HttpCode = (HttpStatusCode)401,
                        Code = "Unauthorized",
                        Description = "A valid access token is required to access this resource"
                    };
            }
        }

        private async Task FinishAuth()
        {
            if (await _web.SendRequestForStatusAsync("HEAD", "me", _accessToken) != HttpStatusCode.Unauthorized)
                await UpdateCurrentUserAsync();
        }

        #endregion

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username)
        {
            return await _web.SendRequestForStatusAsync("HEAD", $"users/{username}", _accessToken, true) == HttpStatusCode.NotFound;
        }

        public Task SendPasswordResetEmailAsync(string usernameOrEmail)
        {
            return _web.SendJsonAsync("PATCH", "users", new ActionRequest() { Value = usernameOrEmail, Action = "send_password_reset_email" }, _accessToken);
        }

        public async Task<User> GetUserAsync(string userId)
        {
            await CheckAuthorization($"users/{userId}");
            return await _web.SendRequestAsync<User>("GET", $"users/{userId}", _accessToken);
        }

        public SelfUser CurrentUser { get; private set; }

        public async Task UpdateCurrentUserAsync()
        {
            Debug.WriteLine("Updating current user...");
            CurrentUser = await _web.SendRequestAsync<SelfUser>("GET", "me", _accessToken);
        }

        public Task CreateAccountAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User feeds

        public Task<GfycatFeed> GetUserGfycatFeedAsync(string userId, int? count = null, string cursor = null)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", $"users/{userId}/gfycats", new { count, cursor }, _accessToken);
        }

        #endregion

        #region Albums

        // todo: implement public album methods

        #endregion

        public async Task<Gfy> GetGfyAsync(string gfycat)
        {
            await CheckAuthorization($"gfycats/{gfycat}");
            return await _web.SendRequestAsync<Gfy>("GET", $"gfycats/{gfycat}", _accessToken);
        }

        #region Creating Gfycats

        /// <summary>
        /// Creates a Gfycat using a remote url and returns the Gfy name
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> CreateGfycatAsync(string remoteUrl, GfyCreationParameters parameters = null)
        {
            parameters = parameters ?? new GfyCreationParameters();
            parameters.FetchUrl = remoteUrl;
            return (await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters, _accessToken)).Gfycat;
        }

        public async Task<GfyStatus> CheckGfyUploadStatusAsync(string gfycat)
        {
            return await _web.SendRequestAsync<GfyStatus>("GET", $"gfycats/fetch/status/{gfycat}");
        }

        public async Task<string> CreateGfycatAsync(Stream data, GfyCreationParameters parameters = null)
        {
            GfyKey uploadKey = await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters ?? new object(), _accessToken);
            _web.BaseAddress = null;
            await _web.SendStreamAsync("POST", "https://filedrop.gfycat.com/", data, uploadKey.Gfycat);
            return uploadKey.Gfycat;
        }

        #endregion

        public Task ShareGfycatOnTwitterAsync(string gfycatId, string postStatus)
        {
            return _web.SendJsonAsync("POST", $"gfycats/{gfycatId}/share/twitter", new { status = postStatus }, _accessToken);
        }

        #region Trending feeds

        public Task<TrendingGfycatFeed> GetTrendingGfycatsAsync(string tag = null, int? count = null, string cursor = null)
        {
            if (!string.IsNullOrWhiteSpace(tag))
                tag = WebUtility.UrlEncode(tag);

            return _web.SendJsonAsync<TrendingGfycatFeed>("GET", "gfycats/trending", new { tagName = tag, count, cursor }, _accessToken);
        }

        public Task<IEnumerable<string>> GetTrendingTagsAsync(int? tagCount = null, string cursor = null)
        {
            return _web.SendJsonAsync<IEnumerable<string>>("GET", "tags/trending", new { tagCount, cursor }, _accessToken);
        }

        public Task<GfycatFeed> GetTrendingTagsPopulatedAsync(int? tagCount = null, int? gfyCount = null, string cursor = null)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", "tags/trending/populated", new { tagCount, gfyCount, cursor }, _accessToken);
        }

        #endregion

        public void Dispose()
        {
            ((IDisposable)_web).Dispose();
        }
    }
}
