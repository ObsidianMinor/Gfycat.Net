using Gfycat.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.OAuth2
{
    /// <summary>
    /// Handles the OAuth flow between the client and Gfycat
    /// </summary>
    public class DefaultAuthenticator : IAuthenticator
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        /// <summary>
        /// The current login user
        /// </summary>
        public string ResourceOwner { get; private set; }

        /// <summary>
        /// The current access token being used for requests
        /// </summary>
        public string AccessToken { get; private set; }
        private Timer _accessTokenTimer;

        /// <summary>
        /// The current refresh token being used for refreshing the access token when it expires
        /// </summary>
        public string RefreshToken { get; private set; }

        internal GfycatApiClient Client { get; }

        /// <summary>
        /// The estimated date and time of the expiration of the current access token
        /// </summary>
        public DateTime? EstimatedAccessTokenExpirationTimeUtc { get; private set; }
        
        /// <summary>
        /// Tells the user the current access token has expired. If using a authentication method that contains a refresh token, the access token will automatically be refreshed
        /// </summary>
        public event EventHandler AccessTokenExpired;

        /// <summary>
        /// The estimated date and time of the expiration of the current refresh token
        /// </summary>
        public DateTime? EstimatedRefreshTokenExpirationTimeUtc { get; private set; }

        private DefaultAuthenticator()
        {
            _accessTokenTimer = new Timer(AccessTokenExpirationCallbackAsync, null, Timeout.Infinite, Timeout.Infinite);
        }
        
        internal DefaultAuthenticator(string clientId, string clientSecret, GfycatApiClient client) : this()
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Client = client;
        }

        /// <summary>
        /// If the current authentication method used includes a refresh token in the response this will refresh both access and refresh tokens
        /// </summary>
        /// <returns></returns>
        public async Task AttemptRefreshTokenAsync(RequestOptions options = null)
        {
            if (CurrentGrantType == AuthenticationGrant.Client)
            {
                await AuthenticateClientAsync();
                return;
            }
            else if (CurrentGrantType == AuthenticationGrant.BrowserImplicitGrant)
                return; // we can't actually refresh this, but if the end user hooks into the AccessTokenExpired event they can handle it
            else
            {
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
                    options);

                Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                AccessToken = auth.AccessToken;
                RefreshToken = auth.RefreshToken;
                SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
            }
        }
        
        /// <summary>
        /// Authenticates this instance using client credentials
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateClientAsync(RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.Client;
            Debug.WriteLine("Performing client credentials authentication...");
            ClientCredentialsAuthResponse auth = await Client.SendJsonAsync<ClientCredentialsAuthResponse>(
                "POST",
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "client_credentials"
                },
                options);

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
        public async Task AuthenticatePasswordAsync(string username, string password, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.Password;
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
                options);
            Debug.WriteLine($"Logged in as {username}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        /// <summary>
        /// Authenticates the client using a Facebook authorization code
        /// </summary>
        /// <param name="authCode">The facebook authorization code</param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateFacebookCodeAsync(string authCode, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.FacebookAuthCode;
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
                options);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            
            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        /// <summary>
        /// Authenticates using a Facebook access code
        /// </summary>
        /// <param name="token">The access token</param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateFacebookTokenAsync(string token, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.FacebookAccessCode;
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
                options);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            
            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        /// <summary>
        /// Gets a Twitter request token for the Twitter authorization flow
        /// </summary>
        /// <returns>An awaitable task that returns a Twitter request token</returns>
        public async Task<string> GetTwitterRequestTokenAsync(RequestOptions options = null)
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
                options);
            return token.RequestToken;
        }

        public static string GetTwitterRequestTokenUrl(string requestToken) => $"https://api.twitter.com/oauth/authenticate?oauth_token={requestToken}";

        /// <summary>
        /// Authenticates a request token and verifier from the Twitter authorization flow
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        public async Task AuthenticateTwitterTokenAsync(string requestToken, string verifier, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.TwitterProvider;
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
                options);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            
            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        /// <summary>
        /// Sets the authorization using an access token and expiration time retrieved from an implicit authorization flow
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenExpiresIn"></param>
        public void Authenticate(string accessToken, int accessTokenExpiresIn)
        {
            CurrentGrantType = AuthenticationGrant.BrowserImplicitGrant;
            Debug.WriteLine($"Recieved access token {accessToken}");
            AccessToken = accessToken;
            SetTimer(accessTokenExpiresIn);
        }

        /// <summary>
        /// Authenticates using an authorization code from the browser authentication flow
        /// </summary>
        /// <param name="code"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task AuthenticateCodeAsync(string code, string redirectUri, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.BrowserAuthCode;
            var auth = await Client.SendJsonAsync<ClientAccountAuthResponse>(
                "POST",
                "oauth/token",
                new BrowserAuthorizationCodeRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "authorization_code",
                    Code = code,
                    RedirectUri = redirectUri
                },
                options);

            Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            ResourceOwner = auth.ResourceOwner;
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            SetTimer(auth.ExpiresIn, auth.RefreshTokenExpiresIn);
        }

        /// <summary>
        /// Authenticates the client using a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task AuthenticateWithRefreshTokenAsync(string refreshToken, RequestOptions options = null)
        {
            CurrentGrantType = AuthenticationGrant.BrowserAuthCode;
            RefreshToken = refreshToken;
            await AttemptRefreshTokenAsync(options);
        }

        /// <summary>
        /// Creates an authorization URL given a state and a redirect URI
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redirectUri"></param>
        /// <param name="codeResponse">True to return a code response (for authorization response), false to return a token response (implicit authorization)</param>
        /// <returns></returns>
        public string GetBrowserAuthUrl(string state, string redirectUri, bool codeResponse) => $"https://gfycat.com/oauth/authorize?client_id={ClientId}&scope=all&state={state}&response_type={(codeResponse ? "code" : "token")}&redirect_uri={redirectUri}";

        private void SetTimer(int time, int? refreshTokenTime = null)
        {
            _accessTokenTimer.Change(TimeSpan.FromSeconds(time), TimeSpan.FromMilliseconds(-1));
            EstimatedAccessTokenExpirationTimeUtc = DateTime.Now.AddSeconds(time);
            Debug.WriteLine($"Client access token expected to expire at {EstimatedAccessTokenExpirationTimeUtc}");
            if(refreshTokenTime.HasValue)
            {
                EstimatedRefreshTokenExpirationTimeUtc = DateTime.Now.AddSeconds(refreshTokenTime.Value);
                Debug.WriteLine($"Client refresh token expected to expire at {EstimatedRefreshTokenExpirationTimeUtc}");
            }
        }

        private async void AccessTokenExpirationCallbackAsync(object state)
        {
            AccessTokenExpired?.Invoke(this, new EventArgs());
            await AttemptRefreshTokenAsync();
        }
    }
}
