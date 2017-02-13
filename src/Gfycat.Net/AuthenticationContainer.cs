using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class AuthenticationContainer
    {
        private AuthenticationGrant _currentGrant;

        public string ClientId { get; }
        public string ClientSecret { get; }

        public string ResourceOwner { get; private set; }

        public string AccessToken { get; private set; }
        private Timer _accessTokenTimer;

        public string RefreshToken { get; private set; }
        private Timer _refreshTokenTimer;

        internal ExtendedHttpClient Client { get; set; }

        public DateTime EstimatedAccessTokenExpirationTime { get; private set; }
        public event EventHandler AccessTokenExpired;

        public DateTime EstimatedRefreshTokenExpirationTime { get; private set; }
        public event EventHandler RefreshTokenExpired;

        public AuthenticationContainer(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public async Task RefreshTokenAsync()
        {
            if (_currentGrant == AuthenticationGrant.Client)
            {
                await AuthenticateAsync();
                return;
            }

            Debug.WriteLine("Refreshing token...");
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new RefreshAuthRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "refresh",
                    RefreshToken = RefreshToken
                }, 
                false);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }
        
        public async Task AuthenticateAsync()
        {
            _currentGrant = AuthenticationGrant.Client;
            Debug.WriteLine("Performing client credentials authentication...");
            var auth = await Client.SendJsonAsync<ClientCredentialsAuthResponse>(
                "POST",
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "client_credentials"
                },
                false);

            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            AccessToken = auth.AccessToken;
            SetTimer(auth.ExpiresIn);
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateAsync(string username, string password)
        {
            _currentGrant = AuthenticationGrant.Password;
            Debug.WriteLine($"Performing client account authentication...");
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new ClientAccountAuthRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "password",
                    Username = username,
                    Password = password
                }, 
                false);
            Debug.WriteLine($"Logged in as {username}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        public async Task AuthenticateFacebookCodeAsync(string authCode)
        {
            _currentGrant = AuthenticationGrant.FacebookAuthCode;
            Debug.WriteLine($"Performing account authentication using Facebook...");
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new FacebookAuthCodeRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "convert_code",
                    Provider = "facebook",
                    AuthCode = authCode
                },
                false);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        public async Task AuthenticateFacebookTokenAsync(string token)
        {
            _currentGrant = AuthenticationGrant.FacebookAccessCode;
            Debug.WriteLine("Performing account authentication using Facebook...");
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new FacebookAuthCodeRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "convert_code",
                    Provider = "facebook",
                    Token = token
                },
                false);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        public async Task<string> GetTwitterRequestTokenAsync()
        {
            Debug.WriteLine("Getting Twitter request token...");
            var token = await Client.SendJsonAsync<TwitterRequestTokenResponse>(
                "POST",
                "oauth/token",
                new
                {
                    grant_type = "request_token",
                    provider = "twitter"
                },
                false);
            return token.RequestToken;
        }

        public async Task AuthenticateTwitterTokenAsync(string requestToken, string verifier)
        {
            _currentGrant = AuthenticationGrant.TwitterProvider;
            Debug.WriteLine("Performing account authentication using Twitter...");
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new TwitterAuthCodeRequest()
                {
                    GrantType = "convert_request_token",
                    Provider = "twitter",
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    Token = requestToken,
                    Verifier = verifier
                },
                false);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        public void Authenticate(string accessToken, int accessTokenExpiresIn, string refreshToken, int refreshTokenExpiresIn, string resourceOwner)
        {
            _currentGrant = AuthenticationGrant.BrowserAuthCode;
            Debug.WriteLine($"Recieved access token {accessToken}");
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ResourceOwner = resourceOwner;
        }

        public async Task AuthenticateAsync(string refreshToken)
        {
            _currentGrant = AuthenticationGrant.BrowserAuthCode;
            RefreshToken = refreshToken;
            await RefreshTokenAsync();
        }

        public string GetBrowserAuthUrl(string state, string redirectUri) => $"https://gfycat.com/oauth/authorize?client_id={ClientId}&scope=all&state={state}&response_type=token&redirect_uri={redirectUri}";

        private void SetTimer(int time, int? refreshTokenTime = null)
        {
            _accessTokenTimer.Change(TimeSpan.FromSeconds(time), TimeSpan.FromMilliseconds(-1));
            EstimatedAccessTokenExpirationTime = DateTime.Now.AddSeconds(time);
            Debug.WriteLine($"Client access token expected to expire at {EstimatedAccessTokenExpirationTime}");
            if(refreshTokenTime.HasValue)
            {
                _accessTokenTimer.Change(TimeSpan.FromSeconds(refreshTokenTime.Value), TimeSpan.FromMilliseconds(-1));
                EstimatedRefreshTokenExpirationTime = DateTime.Now.AddSeconds(refreshTokenTime.Value);
                Debug.WriteLine($"Client refresh token expected to expire at {EstimatedRefreshTokenExpirationTime}");
            }
        }

        private async void AccessTokenExpirationCallbackAsync(object state)
        {
            AccessTokenExpired?.Invoke(this, new EventArgs());
            await RefreshTokenAsync();
        }

        private void RefreshTokenExpirationCallback(object state)
        {
            RefreshTokenExpired?.Invoke(this, new EventArgs());
        }
    }
}
