using Gfycat.API;
using Gfycat.API.Models;
using Gfycat.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents a client for communicating with the Gfycat API
    /// </summary>
    [DebuggerDisplay("Client {ClientId}")]
    public class GfycatClient : ISearchable
    {
        internal GfycatApiClient ApiClient { get; }

        /// <summary>
        /// Specifies the request for the feed should use the default value defined in the client config or the count used in the creation of this feed
        /// </summary>
        public const int UseDefaultFeedCount = -1;
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

        /// <summary>
        /// Gets the object representing the current user
        /// </summary>
        public CurrentUser CurrentUser { get; private set; }

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
                await AuthenticateAsync(options).ConfigureAwait(false);
                return true;
            }
            else if (RefreshToken == null)
                return false;
            else
            {
                Debug.WriteLine("Refreshing token...");
                try
                {
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
                    ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);

                    AccessToken = auth.AccessToken;
                    RefreshToken = auth.RefreshToken;
                    options.UseAccessToken = true;
                    await CurrentUser.UpdateAsync(options).ConfigureAwait(false);
                }
                catch(GfycatException e) when (e.HttpCode == HttpStatusCode.Unauthorized && e.Code == "InvalidRefreshToken")
                {
                    return false;
                }
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
            ClientCredentialsAuthResponse auth = await response.ReadAsJsonAsync<ClientCredentialsAuthResponse>(ApiClient.Config).ConfigureAwait(false);
            
            AccessToken = auth.AccessToken;
            CurrentUser = null;
            _clientAuth = true;
        }

        /// <summary>
        /// Allows the owner of this client to log in with their username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="options">The options for this request</param>
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
            ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
            
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            options.UseAccessToken = true;
            CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
            _clientAuth = false;
        }

        /// <summary>
        /// Retrieves a Twitter request token for the Twitter Request token auth flow
        /// </summary>
        /// <param name="options">The options for this request</param>
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
            TwitterRequestTokenResponse auth = await response.ReadAsJsonAsync<TwitterRequestTokenResponse>(ApiClient.Config).ConfigureAwait(false);
            return auth.RequestToken;
        }

        /// <summary>
        /// Allows authentication with either a facebook access token or facebook auth code
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="options">The options for this request</param>
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
                        ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
                        
                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                        options.UseAccessToken = true;
                        CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
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
                        ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
                        
                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                        options.UseAccessToken = true;
                        CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
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
        /// <param name="options">The options for this request</param>
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
                        ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
                        
                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                        options.UseAccessToken = true;
                        CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
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
                        ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
                        
                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                        options.UseAccessToken = true;
                        CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
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
                        ClientAccountAuthResponse auth = await response.ReadAsJsonAsync<ClientAccountAuthResponse>(ApiClient.Config).ConfigureAwait(false);
                        
                        AccessToken = auth.AccessToken;
                        RefreshToken = auth.RefreshToken;
                        options.UseAccessToken = true;
                        CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
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
        /// <param name="options">Sets the options for this request</param>
        /// <returns></returns>
        /// <exception cref="GfycatException">If verifyToken is true, this will attempt to get the current user which will return 401 unauthorized if the access token is invalid</exception>
        public async Task AuthenticateAsync(string accessToken, RequestOptions options = null)
        {
            AccessToken = accessToken;

            CurrentUser = await GetCurrentUserAsync(options).ConfigureAwait(false);
        }

        #endregion

        #region Users

        /// <summary>
        /// Returns a bool for whether or not a user account exists on that username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<bool> GetUserExistsAsync(string username, RequestOptions options = null)
        {
            return (await ApiClient.GetUsernameStatusAsync(username, options).ConfigureAwait(false)).IsSuccessfulStatus();
        }

        /// <summary>
        /// Sends a password reset email for the user with the specified username or email address
        /// </summary>
        /// <param name="usernameOrEmail"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task SendPasswordResetEmailAsync(string usernameOrEmail, RequestOptions options = null)
        {
            await ApiClient.SendPasswordResetEmailAsync(usernameOrEmail, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Tries to get the user with the given Id. If the user isn't found, this throws a <see cref="GfycatException"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        /// <exception cref="GfycatException"></exception>
        public async Task<User> GetUserAsync(string userId, RequestOptions options = null)
        {
            API.Models.User userModel = await ApiClient.GetUserAsync(userId, options).ConfigureAwait(false);
            return User.Create(this, userModel);
        }

        /// <summary>
        /// Tries to get the user with the given ID. If the user isn't found, this returns null
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<User> TryGetUserAsync(string userId, RequestOptions options = null) 
            => (await GetUserExistsAsync(userId, options).ConfigureAwait(false)) ? await GetUserAsync(userId, options).ConfigureAwait(false) : null;

        /// <summary>
        /// Attempts to get the current user
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<CurrentUser> GetCurrentUserAsync(RequestOptions options = null)
        {
            API.Models.CurrentUser currentUserModel = await ApiClient.GetCurrentUserAsync(options).ConfigureAwait(false);
            return CurrentUser.Create(this, currentUserModel);
        }

        /// <summary>
        /// Creates a new user account using the specified username, password, and email
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="loginWhenComplete"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task CreateAccountAsync(string username, string password, string email = null, bool loginWhenComplete = true, RequestOptions options = null)
        {
            ClientAccountAuthResponse response = await ApiClient.CreateAccountAsync(new AccountCreationRequest()
            {
                Username = username,
                Password = password,
                Email = email
            }, options).ConfigureAwait(false);

            if (loginWhenComplete)
            {
                AccessToken = response.AccessToken;
                RefreshToken = response.RefreshToken;
            }
        }

        /// <summary>
        /// Creates a new account using the specified username, provider token, password, and email
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tokenType"></param>
        /// <param name="token"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="loginWhenComplete"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
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

            ClientAccountAuthResponse response = await ApiClient.CreateAccountAsync(request, options).ConfigureAwait(false);

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
        /// <param name="overrideInvalidNameDetection">Forces the method to fetch the specified gfy even if it's detected as an invalid gfy name</param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<Gfy> GetGfyAsync(string gfycat, bool overrideInvalidNameDetection = false, RequestOptions options = null)
        {
            if (!overrideInvalidNameDetection && !IsValidGfyName(gfycat))
                return null;

            if (_clientAuth) // can I just say how much implicit auth is a pain the ass?
            {
                GfyResponse response = await ApiClient.GetGfyAsync(gfycat, options).ConfigureAwait(false);
                if (response == null)
                    return null;

                return Gfy.Create(this, response.GfyItem);
            }
            else
            {
                FullGfyResponse response = await ApiClient.GetFullGfyAsync(gfycat, options).ConfigureAwait(false);
                if (response == null)
                    return null;

                return Gfy.Create(this, response.Gfy);
            }
        }
        /// <summary>
        /// Returns whether the provided url string is a valid Gfycat url pointing to a valid gfy name
        /// </summary>
        /// <param name="gfycatUrl"></param>
        /// <param name="gfyName"></param>
        /// <returns></returns>
        public static bool IsValidGfyUrl(string gfycatUrl, out string gfyName)
        {
            if (gfycatUrl == null || !Uri.TryCreate(gfycatUrl, UriKind.Absolute, out Uri result) || !result.Scheme.Contains("http") || !result.Host.EndsWith("gfycat.com") || result.LocalPath == "/")
            {
                gfyName = null;
                return false;
            }
            else
            {
                string lastSegment = result.Segments.LastOrDefault();
                int fileExtensionPos = lastSegment.LastIndexOf('.');
                if (fileExtensionPos != -1)
                    lastSegment = lastSegment.Remove(fileExtensionPos);
                int infoPos = lastSegment.LastIndexOf('-'); // get the position of the file info portion, for instance in the name YellowIdenticalEuropeanpolecat-poster.jpg
                if (infoPos != -1)
                    lastSegment = lastSegment.Remove(infoPos);
                
                gfyName = IsValidGfyName(lastSegment) ? lastSegment : null;
                return gfyName != null;
            }
        }
        /// <summary>
        /// Checks whether the provided gfy name is valid
        /// </summary>
        /// <param name="gfyName"></param>
        /// <returns></returns>
        public static bool IsValidGfyName(string gfyName)
        {
            if (gfyName.Equals("BrutalSavageRekt", StringComparison.OrdinalIgnoreCase))
                return true;

            if (gfyName.Length > Utils.GfyFinalSegmentMaxLength)
                return false;

            gfyName = gfyName.ToLowerInvariant();
            //adjectives
            string match1 = "";
            string match2 = "";
            string match3 = "";

            while (Utils.HasPartial(gfyName, Utils.GfyAdjectives, ref match1))
                while (Utils.HasPartial(gfyName.Substring(match1.Length), Utils.GfyAdjectives, ref match2))
                    while (Utils.HasPartial(gfyName.Substring(match1.Length + match2.Length), Utils.GfyAnimals, ref match3))
                        if (match1.Length + match2.Length + match3.Length == gfyName.Length)
                            return true;

            return false;
            
            /* there's always a way to do it without goto, I will now keep this code here as a warning to not use goto
            begin:
            if (Utils.HasPartial(lastSegment, Utils.GfyAdjectives, ref match1))
            {
                second:
                if (Utils.HasPartial(lastSegment.Substring(match1.Length), Utils.GfyAdjectives, ref match2))
                {
                    if (Utils.HasPartial(lastSegment.Substring(match1.Length + match2.Length), Utils.GfyAnimals, ref match3))
                        return await GetGfyAsync(lastSegment, options);
                    else
                        goto second;
                }
                else 
                    goto begin;
            }

            return null;
            */
        }

        /// <summary>
        /// Attempts to get info for a single Gfy using a url string. If the URI isn't in a valid format or the gfy does not exist, this returns null
        /// </summary>
        /// <param name="gfycatUrl">The gfycat url</param>
        /// <param name="overrideInvalidNameDetection">Forces the method to fetch the specified gfy even if it's detected as an invalid gfy name</param>
        /// <param name="options">Optional request parameters</param>
        /// <returns>An awaitable task that returns a Gfy</returns>
        public async Task<Gfy> GetGfyFromUrlAsync(string gfycatUrl, bool overrideInvalidNameDetection = false, RequestOptions options = null)
        {
            return IsValidGfyUrl(gfycatUrl, out string gfy) ? await GetGfyAsync(gfy, overrideInvalidNameDetection, options).ConfigureAwait(false) : null;
        }

        #region Creating Gfycats

        /// <summary>
        /// Creates a Gfy using a remote url and returns the Gfy status
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyStatus> CreateGfyAsync(string remoteUrl, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey key = await ApiClient.CreateGfyFromFetchUrlAsync(remoteUrl, parameters ?? new GfyCreationParameters(), options).ConfigureAwait(false);
            await Task.Delay(500).ConfigureAwait(false);
            return await GetGfyUploadStatusAsync(key.Gfycat, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the upload status of the specified gfy
        /// </summary>
        /// <param name="gfycat"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyStatus> GetGfyUploadStatusAsync(string gfycat, RequestOptions options = null)
        {
            Status status = await ApiClient.GetGfyStatusAsync(gfycat, options).ConfigureAwait(false);

            if (status.Task == UploadTask.Error)
                throw new GfycatException(status.ErrorDescription);

            return new GfyStatus(this, status);
        }

        /// <summary>
        /// Creates a Gfycat using the specified stream and upload parameters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parameters"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<GfyStatus> CreateGfyAsync(Stream data, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey uploadKey = await ApiClient.GetUploadKeyAsync(parameters.CreateModel(), options).ConfigureAwait(false);
            await ApiClient.PostGfyStreamAsync(uploadKey, data, options).ConfigureAwait(false);
            await Task.Delay(500).ConfigureAwait(false);

            return await GetGfyUploadStatusAsync(uploadKey.Gfycat, options).ConfigureAwait(false);
        }

        #endregion

        #region Trending feeds

        /// <summary>
        /// Retrieves trending gfys for the specified tag. If no tag is specified, the "Trending" tag is used
        /// </summary>
        /// <param name="count"></param>
        /// <param name="tag"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<TaggedGfyFeed> GetTrendingGfysAsync(string tag = "Trending", int count = UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, ApiClient.Config.DefaultFeedItemCount);
            return TaggedGfyFeed.Create(this, count, (await ApiClient.GetTrendingFeedAsync(tag, count, null, options).ConfigureAwait(false)), options);
        }

        /// <summary>
        /// Returns an enumerable of all trending tags
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetTrendingTagsAsync(RequestOptions options = null)
        {
            return await ApiClient.GetTrendingTagsAsync(null, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves trending tags populated with gfys
        /// </summary>
        /// <param name="tagCount"></param>
        /// <param name="gfyCount"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<PopulatedTagFeed> GetTrendingTagsPopulatedAsync(int tagCount = UseDefaultFeedCount, int gfyCount = UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref tagCount, ApiClient.Config.DefaultFeedItemCount);
            Utils.UseDefaultIfSpecified(ref gfyCount, ApiClient.Config.DefaultFeedItemCount);
            return PopulatedTagFeed.Create(this, gfyCount, tagCount, options, await ApiClient.GetTrendingTagsPopulatedAsync(null, tagCount, gfyCount, options).ConfigureAwait(false));
        }

        #endregion

        /// <summary>
        /// Retrieves a feed of all reaction tag feeds in the specified language. The default language is english
        /// </summary>
        /// <param name="count"></param>
        /// <param name="language"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<ReactionTagsFeed> GetReactionGfysAsync(ReactionLanguage language = ReactionLanguage.English, int count = UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, ApiClient.Config.DefaultFeedItemCount);
            return ReactionTagsFeed.Create(this, count, options, await ApiClient.GetReactionGifsPopulatedAsync(language, count, null, options).ConfigureAwait(false), language);
        }

        /// <summary>
        /// Searches the Gfycat website using the provided search text
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="count"></param>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        public async Task<SearchFeed> SearchAsync(string searchText, int count = UseDefaultFeedCount, RequestOptions options = null)
        {
            Utils.UseDefaultIfSpecified(ref count, ApiClient.Config.DefaultFeedItemCount);
            return SiteSearchFeed.Create(this, await ApiClient.SearchSiteAsync(searchText, count, null, options).ConfigureAwait(false), count, searchText, options);
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
