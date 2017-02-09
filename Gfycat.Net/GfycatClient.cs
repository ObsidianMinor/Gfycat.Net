using System;
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
        
        public SelfUser CurrentUser { get; private set; }

        public GfycatClient(string clientId, string clientSecret)
        {
            _web = new HttpClient() { BaseAddress = new Uri(_startEndpoint) };

            Debug.WriteLine($"Creating client with ID \"{clientId}\"");
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        #region Authentication

        public async Task RefreshToken()
        {
            Debug.WriteLine("Refreshing token...");
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

            Debug.WriteLine($"Logged in as {response.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {response.AccessToken}");

            _refreshToken = response.RefreshToken;
            _accessToken = response.AccessToken;
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

        public async void Authenticate(string accessToken, string refreshToken)
        {
            Debug.WriteLine($"Recieved access token {accessToken}");
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            await FinishAuth();
        }

        private async Task FinishAuth()
        {
            if(await _web.SendRequestForStatusAsync("HEAD", "me", _accessToken) != HttpStatusCode.Unauthorized)
                await UpdateCurrentUser();
        }

        public async Task UpdateCurrentUser()
        {
            Debug.WriteLine("Updating current user...");
            CurrentUser = await _web.SendRequestAsync<SelfUser>("GET", "me", _accessToken);
        }

        internal async Task CheckAuthorization(string endpoint)
        {
            if (await _web.SendRequestForStatusAsync("HEAD", endpoint, _accessToken) == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                if (await _web.SendRequestForStatusAsync("HEAD", endpoint, _accessToken) == HttpStatusCode.Unauthorized)
                    throw new GfycatException()
                    {
                        HttpCode = (HttpStatusCode)401,
                        Code = "Unauthorized",
                        Description = "A valid access token is required to access this resource"
                    };
            }
        }

        public string GetBrowserAuthUrl(string state, string redirectUri) => $"https://gfycat.com/oauth/authorize?client_id={_clientId}&scope=all&state={state}&response_type=token&redirect_uri={redirectUri}";

        #endregion

        public async Task<GfyStatus> CheckGfyUploadStatus(string gfycat)
        {
            return await _web.SendRequestAsync<GfyStatus>("GET", $"gfycats/fetch/status/{gfycat}");
        }

        public async Task<Gfy> GetGfy(string gfycat)
        {
            await CheckAuthorization($"gfycats/{gfycat}");
            return await _web.SendRequestAsync<Gfy>("GET", $"gfycats/{gfycat}", _accessToken);
        }

        public async Task<User> GetUser(string userId)
        {
            await CheckAuthorization($"users/{userId}");
            return await _web.SendRequestAsync<User>("GET", $"users/{userId}", _accessToken);
        }

        public Task CreateAccount()
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateGfycat(Stream data, GfyCreationParameters parameters = null)
        {
            //GfyKey uploadKey = await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters ?? new object(), _accessToken);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Gfycat using a remote url and returns the Gfy name
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> CreateGfycat(string remoteUrl, GfyCreationParameters parameters = null)
        {
            parameters = parameters ?? new GfyCreationParameters();
            parameters.FetchUrl = remoteUrl;
            return (await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters, _accessToken)).Gfycat;
        }

        public void Dispose()
        {
            ((IDisposable)_web).Dispose();
        }

        public Task<GfycatFeed> GetUserGfycatFeed(string userId)
        {
            return _web.SendRequestAsync<GfycatFeed>("GET", "users/{userId}/gfycats", _accessToken);
        }

        public Task<GfycatFeed> GetUserGfycatFeed(string userId, int count)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", $"users/{userId}/gfycats", new { count }, _accessToken);
        }

        public Task<GfycatFeed> GetUserGfycatFeed(string userId, int count, string cursor)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", $"users/{userId}/gfycats", new { count, cursor }, _accessToken);
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
            return await _web.SendRequestForStatusAsync("HEAD", $"users/{username}", _accessToken, true) == HttpStatusCode.NotFound;
        }
    }
}
