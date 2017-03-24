using Gfycat.API;
using Gfycat.API.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Gfycat.Rest;
using System;

namespace Gfycat
{
    [DebuggerDisplay("Client {ClientId}")]
    public class GfycatClient : ISearchable
    {
        internal GfycatApiClient ApiClient { get; }

        public GfycatClient(string clientId, string clientSecret) : this(new GfycatClientConfig(clientId, clientSecret))
        {
        }

        public GfycatClient(GfycatClientConfig config)
        {
            ApiClient = new GfycatApiClient(this, config);

            Debug.WriteLine($"Client created with ID \"{config.ClientId}\"");
        }

        #region Auth methods

        public string ClientId => ApiClient.Config.ClientId;
        private string ClientSecret => ApiClient.Config.ClientSecret;

        public string AccessToken { get; private set; }

        /// <summary>
        /// The current refresh token being used for refreshing the access token when it expires
        /// </summary>
        public string RefreshToken { get; private set; }

        private IRestClient RestClient => ApiClient.RestClient;

        /// <summary>
        /// If the current authentication method used includes a refresh token in the response this will refresh both access and refresh tokens
        /// </summary>
        /// <returns></returns>
        public async Task RefreshTokenAsync(string providedRefreshToken = null, RequestOptions options = null)
        {
            if (!string.IsNullOrWhiteSpace(providedRefreshToken))
                RefreshToken = providedRefreshToken;

            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            if (RefreshToken == null)
            {
                await AuthenticateAsync(options);
                return;
            }
            else
            {
                Debug.WriteLine("Refreshing token...");
                RestResponse response = await ApiClient.SendJsonAsync(
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
                ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                AccessToken = auth.AccessToken;
                RefreshToken = auth.RefreshToken;
            }
        }

        /// <summary>
        /// Authenticates this instance using client credentials
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateAsync(RequestOptions options = null)
        {
            Debug.WriteLine("Performing client credentials authentication...");
            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            RestResponse response = await ApiClient.SendJsonAsync(
                "POST",
                "oauth/token",
                new ClientCredentialsAuthRequest()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    GrantType = "client_credentials"
                },
                options);
            ClientCredentialsAuthResponse auth = response.ReadAsJson<ClientCredentialsAuthResponse>(ApiClient.Config);

            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            AccessToken = auth.AccessToken;
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>An awaitable task</returns>
        public async Task AuthenticateAsync(string username, string password, RequestOptions options = null)
        {
            Debug.WriteLine($"Performing client account authentication...");
            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            RestResponse response = await ApiClient.SendJsonAsync(
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
            ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

            Debug.WriteLine($"Logged in as {username}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
        }

        public async Task<string> GetTwitterRequestTokenAsync(RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            RestResponse response = await ApiClient.SendJsonAsync(
                "POST",
                "oauth/token",
                new
                {
                    grant_type = "request_token",
                    provider = "twitter"
                },
                options);
            TwitterRequestTokenResponse auth = response.ReadAsJson<TwitterRequestTokenResponse>(ApiClient.Config);
            return auth.RequestToken;
        }

        public async Task AuthenticateAsync(TokenType type, string token, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            switch (type)
            {
                case TokenType.FacebookAccessToken:
                    {
                        RestResponse response = await ApiClient.SendJsonAsync(
                            "POST",
                            "oauth/token",
                            new FacebookAuthCodeRequest()
                            {
                                ClientSecret = ClientSecret,
                                ClientId = ClientId,
                                GrantType = "convert_code",
                                Provider = "facebook",
                                AuthCode = token
                            },
                            options);
                        ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                        Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                        Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                    }
                    break;
                case TokenType.FacebookAuthCode:
                    {
                        RestResponse response = await ApiClient.SendJsonAsync(
                            "POST",
                            "oauth/token",
                            new FacebookAuthCodeRequest()
                            {
                                ClientSecret = ClientSecret,
                                ClientId = ClientId,
                                GrantType = "convert_token",
                                Provider = "facebook",
                                Token = token
                            },
                            options);
                        ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                        Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                        Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The provided token type does not accept one token parameter");
            }
        }

        /// <summary>
        /// Authenticates using a browser auth code and redirect uri, twitter token and secret, or twitter token and verifier
        /// </summary>
        /// <param name="type">The type of the provided tokens</param>
        /// <param name="tokenOrCode">A twitter token or browser auth code</param>
        /// <param name="verifierSecretRedirectUri">A twitter secret, verifier, or browser redirect uri</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task AuthenticateAsync(TokenType type, string tokenOrCode, string verifierSecretRedirectUri, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            switch (type)
            {
                case TokenType.AuthorizationCode:
                    {
                        RestResponse response = await ApiClient.SendJsonAsync(
                            "POST",
                            "oauth/token",
                            new BrowserAuthorizationCodeRequest()
                            {
                                ClientSecret = ClientSecret,
                                ClientId = ClientId,
                                GrantType = "authorization_code",
                                Code = tokenOrCode,
                                RedirectUri = verifierSecretRedirectUri
                            },
                            options);
                        ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                        Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                        Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                    }
                    break;
                case TokenType.TwitterTokenSecret:
                    {
                        RestResponse response = await ApiClient.SendJsonAsync(
                            "POST",
                            "oauth/token",
                            new TwitterAuthCodeRequest()
                            {
                                ClientSecret = ClientSecret,
                                ClientId = ClientId,
                                GrantType = "convert_request_token",
                                Provider = "twitter",
                                Token = tokenOrCode,
                                Verifier = verifierSecretRedirectUri
                            },
                            options);
                        ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                        Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                        Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                    }
                    break;
                case TokenType.TwitterTokenVerifier:
                    {
                        RestResponse response = await ApiClient.SendJsonAsync(
                            "POST",
                            "oauth/token",
                            new TwitterAuthCodeRequest()
                            {
                                ClientSecret = ClientSecret,
                                ClientId = ClientId,
                                GrantType = "convert_token",
                                Provider = "twitter",
                                Token = tokenOrCode,
                                Secret = verifierSecretRedirectUri
                            },
                            options);
                        ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                        Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                        Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The provided token type does not accept two token parameters");
            }
        }

        public async Task AuthenticateAsync(string accessToken, bool verifyToken = true)
        {
            AccessToken = accessToken;

            if (verifyToken)
                await GetCurrentUserAsync(); // throws if the access token is invalid
        }
        #endregion

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username, RequestOptions options = null)
        {
            return (await ApiClient.GetUsernameStatusAsync(username, options)) == HttpStatusCode.NotFound;
        }

        public async Task SendPasswordResetEmailAsync(string usernameOrEmail, RequestOptions options = null)
        {
            await ApiClient.SendPasswordResetEmailAsync(usernameOrEmail, options);
        }

        public async Task<User> GetUserAsync(string userId, RequestOptions options = null)
        {
            API.Models.User userModel = await ApiClient.GetUserAsync(userId, options);
            return User.Create(this, userModel);
        }

        public async Task<CurrentUser> GetCurrentUserAsync(RequestOptions options = null)
        {
            API.Models.CurrentUser currentUserModel = await ApiClient.GetCurrentUserAsync(options);
            return CurrentUser.Create(this, currentUserModel);
        }

        #endregion

        public async Task<Gfy> GetGfyAsync(string gfycat, RequestOptions options = null)
        {
            GfyResponse response = await ApiClient.GetGfyAsync(gfycat, options);
            return Gfy.Create(this, response.GfyItem);
        }

        #region Creating Gfycats

        /// <summary>
        /// Creates a Gfycat using a remote url and returns the Gfy name
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> CreateGfyAsync(string remoteUrl, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey key = await ApiClient.CreateGfyFromFetchUrlAsync(remoteUrl, parameters ?? new GfyCreationParameters(), options);
            return key.Gfycat;
        }

        public async Task<GfyStatus> GetGfyUploadStatusAsync(string gfycat, RequestOptions options = null)
        {
            Status status = await ApiClient.GetGfyStatusAsync(gfycat, options);

            if (status.Task == UploadTask.Error)
                throw new GfycatException(HttpStatusCode.OK.ToString(), status.ErrorDescription, HttpStatusCode.OK);

            return new GfyStatus(this, status);
        }

        public async Task<string> CreateGfyAsync(Stream data, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey uploadKey = await ApiClient.GetUploadKeyAsync(parameters, options);
            await ApiClient.PostGfyStreamAsync(uploadKey, data, options);

            return uploadKey.Gfycat;
        }

        #endregion

        #region Trending feeds

        public async Task<TaggedGfyFeed> GetTrendingGfysAsync(string tag = null, RequestOptions options = null)
        {
            return TaggedGfyFeed.Create(this, (await ApiClient.GetTrendingFeedAsync(tag, null, options)), options);
        }

        public async Task<IEnumerable<string>> GetTrendingTagsAsync(RequestOptions options = null)
        {
            return await ApiClient.GetTrendingTagsAsync(null, options);
        }

        public async Task<PopulatedTagFeed> GetTrendingTagsPopulatedAsync(RequestOptions options = null)
        {
            return PopulatedTagFeed.Create(this, options, await ApiClient.GetTrendingTagsPopulatedAsync(null, options));
        }

        #endregion

        // supposedly in testing. hhhhhhhhhhhhhhhhmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
        public async Task<GfyFeed> SearchAsync(string searchText, RequestOptions options = null)
        {
            return SiteSearchFeed.Create(this, await ApiClient.SearchSiteAsync(searchText, null, options), searchText, options);
        }

        #region Extras

        private static readonly IReadOnlyDictionary<TokenType, string> _tokensToString = new Dictionary<TokenType, string>()
        {
            { TokenType.FacebookAuthCode, "auth_code" },
            { TokenType.FacebookAccessToken, "access_token" },
            { TokenType.TwitterTokenSecret, "secret" },
            { TokenType.TwitterTokenVerifier, "verifier" }
        };

        public static string GetTwitterRequestTokenUrl(string requestToken) => $"https://api.twitter.com/oauth/authenticate?oauth_token={requestToken}";

        /// <summary>
        /// Creates an authorization URL given a state and a redirect URI
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redirectUri"></param>
        /// <param name="codeResponse">True to return a code response (for authorization response), false to return a token response (implicit authorization)</param>
        /// <returns></returns>
        public string GetBrowserAuthUrl(string state, string redirectUri, bool codeResponse) => $"https://gfycat.com/oauth/authorize?client_id={ClientId}&scope=all&state={state}&response_type={(codeResponse ? "code" : "token")}&redirect_uri={redirectUri}";

        #endregion

    }
}
