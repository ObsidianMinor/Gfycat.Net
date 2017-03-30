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
    /// <summary>
    /// Represents a client for communicating with the Gfycat API
    /// </summary>
    [DebuggerDisplay("Client {ClientId}")]
    public class GfycatClient : ISearchable
    {
        internal GfycatApiClient ApiClient { get; }

        const string TwitterProvider = "twitter";
        const string FacebookProvider = "facebook";
        bool _clientAuth = true;

        /// <summary>
        /// Creates a <see cref="GfycatClient"/> using the provided client ID and client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public GfycatClient(string clientId, string clientSecret) : this(new GfycatClientConfig(clientId, clientSecret))
        {
        }

        /// <summary>
        /// Creates a <see cref="GfycatClient"/> using the provided <see cref="GfycatClientConfig"/>
        /// </summary>
        /// <param name="config"></param>
        public GfycatClient(GfycatClientConfig config)
        {
            ApiClient = new GfycatApiClient(this, config);

            Debug.WriteLine($"Client created with ID \"{config.ClientId}\"");
        }

        #region Auth methods

        /// <summary>
        /// This client's ID provided during construction
        /// </summary>
        public string ClientId => ApiClient.Config.ClientId;
        internal string ClientSecret => ApiClient.Config.ClientSecret;

        /// <summary>
        /// The current access token being used for all requests
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// The current refresh token being used for refreshing the access token when it expires
        /// </summary>
        public string RefreshToken { get; private set; }

        private IRestClient RestClient => ApiClient.RestClient;

        /// <summary>
        /// Attempts to refresh the access token using the current refresh token or with a provided access token. If the current refresh token is null or an refresh token isn't provided, this will perform client credential authentication
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RefreshTokenAsync(string providedRefreshToken = null, RequestOptions options = null)
        {
            if (!string.IsNullOrWhiteSpace(providedRefreshToken))
                RefreshToken = providedRefreshToken;

            options = options ?? RequestOptions.CreateFromDefaults(ApiClient.Config);
            options.UseAccessToken = false;
            if (_clientAuth)
            {
                await AuthenticateAsync(options);
                return true;
            }
            else if (RefreshToken == null)
                return false;
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
                    options).ConfigureAwait(false);
                ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

                Debug.WriteLine($"Logged in as {auth.ResourceOwner}");
                Debug.WriteLine($"Recieved access token {auth.AccessToken}");

                AccessToken = auth.AccessToken;
                RefreshToken = auth.RefreshToken;
                return true;
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
                options).ConfigureAwait(false);
            ClientCredentialsAuthResponse auth = response.ReadAsJson<ClientCredentialsAuthResponse>(ApiClient.Config);

            Debug.WriteLine($"Recieved access token {auth.AccessToken}");
            AccessToken = auth.AccessToken;
            _clientAuth = true;
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="options"></param>
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
                options).ConfigureAwait(false);
            ClientAccountAuthResponse auth = response.ReadAsJson<ClientAccountAuthResponse>(ApiClient.Config);

            Debug.WriteLine($"Logged in as {username}");
            Debug.WriteLine($"Recieved access token {auth.AccessToken}");

            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            _clientAuth = false;
        }

        /// <summary>
        /// Retrieves a Twitter request token for the Twitter Request token auth flow
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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
                options).ConfigureAwait(false);
            TwitterRequestTokenResponse auth = response.ReadAsJson<TwitterRequestTokenResponse>(ApiClient.Config);
            return auth.RequestToken;
        }

        /// <summary>
        /// Allows authentication with either a facebook access token or facebook auth code
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
                            options).ConfigureAwait(false);
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
                            options).ConfigureAwait(false);
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
            _clientAuth = false;
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
                            options).ConfigureAwait(false);
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
                            options).ConfigureAwait(false);
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
                            options).ConfigureAwait(false);
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
            _clientAuth = false;
        }

        /// <summary>
        /// Authenticates using an given access token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="verifyToken"></param>
        /// <returns></returns>
        /// <exception cref="GfycatException">If verifyToken is true, this will attempt to get the current user which will return 401 unauthorized if the access token is invalid</exception>
        public async Task AuthenticateAsync(string accessToken, bool verifyToken = true)
        {
            AccessToken = accessToken;

            if (verifyToken)
                await GetCurrentUserAsync(); // throws if the access token is invalid
        }

        #endregion

        #region Users

        public async Task<bool> GetUserExistsAsync(string username, RequestOptions options = null)
        {
            return (await ApiClient.GetUsernameStatusAsync(username, options)) == HttpStatusCode.OK;
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

        public async Task<User> TryGetUserAsync(string userId, RequestOptions options = null) 
            => (await GetUserExistsAsync(userId, options)) ? await GetUserAsync(userId, options) : null;

        public async Task<CurrentUser> GetCurrentUserAsync(RequestOptions options = null)
        {
            API.Models.CurrentUser currentUserModel = await ApiClient.GetCurrentUserAsync(options);
            return CurrentUser.Create(this, currentUserModel);
        }

        public async Task CreateAccountAsync(string username, string password, string email = null, bool loginWhenComplete = true, RequestOptions options = null)
        {
            ClientAccountAuthResponse response = await ApiClient.CreateAccountAsync(new AccountCreationRequest()
            {
                Username = username,
                Password = password,
                Email = email
            }, options);

            if (loginWhenComplete)
            {
                AccessToken = response.AccessToken;
                RefreshToken = response.RefreshToken;
            }
        }

        public async Task CreateAccountAsync(string username, AccountTokenType tokenType, string token, string password = null, string email = null, bool loginWhenComplete = true, RequestOptions options = null)
        {
            AccountCreationRequest request = new AccountCreationRequest()
            {
                Username = username,
                Password = password,
                Email = email,
            };

            switch(tokenType)
            {
                case AccountTokenType.FacebookAuthCode:
                    request.Provider = FacebookProvider;
                    request.AuthCode = token;
                    break;
                case AccountTokenType.FacebookAccessToken:
                    request.Provider = FacebookProvider;
                    request.AccessToken = token;
                    break;
                case AccountTokenType.TwitterToken:
                    request.Provider = TwitterProvider;
                    request.Token = token;
                    break;
                case AccountTokenType.TwitterVerifier:
                    request.Provider = TwitterProvider;
                    request.Verifier = token;
                    break;
                case AccountTokenType.TwitterOauthToken:
                    request.Provider = TwitterProvider;
                    request.OauthToken = token;
                    break;
                case AccountTokenType.TwitterOauthTokenSecret:
                    request.Provider = TwitterProvider;
                    request.OauthTokenSecret = token;
                    break;
            }

            ClientAccountAuthResponse response = await ApiClient.CreateAccountAsync(request, options);

            if (loginWhenComplete)
            {
                AccessToken = response.AccessToken;
                RefreshToken = response.RefreshToken;
            }
        }
        
        #endregion

        /// <summary>
        /// Gets info for a single Gfy
        /// </summary>
        /// <param name="gfycat"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<Gfy> GetGfyAsync(string gfycat, RequestOptions options = null)
        {
            GfyResponse response = await ApiClient.GetGfyAsync(gfycat, options);
            if (response == null)
                return null;

            return Gfy.Create(this, response.GfyItem);
        }

        /// <summary>
        /// Attempts to get info for a single Gfy using a url string. If the URI isn't in a valid format or the gfy does not exist, this returns null
        /// </summary>
        /// <param name="gfycatUrl">The gfycat url</param>
        /// <param name="options">Optional request parameters</param>
        /// <returns>An awaitable Gfy</returns>
        public async Task<Gfy> GetGfyFromUrlAsync(string gfycatUrl, RequestOptions options = null)
        {
            if (!Uri.TryCreate(gfycatUrl, UriKind.Absolute, out Uri result) || result.Host != "gfycat.com" || result.LocalPath == "/")
                return null;
            else
                return await GetGfyAsync(result.Segments.LastOrDefault(), options);
        }

        #region Creating Gfycats

        /// <summary>
        /// Creates a Gfycat using a remote url and returns the Gfy name
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<GfyStatus> CreateGfyAsync(string remoteUrl, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey key = await ApiClient.CreateGfyFromFetchUrlAsync(remoteUrl, parameters ?? new GfyCreationParameters(), options);
            return await GetGfyUploadStatusAsync(key.Gfycat, options);
        }

        public async Task<GfyStatus> GetGfyUploadStatusAsync(string gfycat, RequestOptions options = null)
        {
            Status status = await ApiClient.GetGfyStatusAsync(gfycat, options);

            if (status.Task == UploadTask.Error)
                throw new GfycatException(status.ErrorDescription);

            return new GfyStatus(this, status);
        }

        public async Task<GfyStatus> CreateGfyAsync(Stream data, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey uploadKey = await ApiClient.GetUploadKeyAsync(parameters.CreateModel(), options);
            await ApiClient.PostGfyStreamAsync(uploadKey, data, options);
            await Task.Delay(500);

            return await GetGfyUploadStatusAsync(uploadKey.Gfycat, options);
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

        public async Task<ReactionTagsFeed> GetReactionGfysAsync(ReactionLanguage language = ReactionLanguage.English, RequestOptions options = null)
        {
            return ReactionTagsFeed.Create(this, options, await ApiClient.GetReactionGifsPopulatedAsync(language, null, options), language);
        }

        /// <summary>
        /// Searches the Gfycat website using the provided search text
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Formats a request token into a Twitter oauth url
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns></returns>
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
