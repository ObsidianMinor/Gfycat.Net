using Gfycat.OAuth2;
using Gfycat.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient
    {
        internal IRestClient RestClient { get; }
        internal GfycatClientConfig Config { get; }
        public AuthenticationContainer Authentication { get; }

        public GfycatClient(string clientId, string clientSecret) : this(clientId, clientSecret, new GfycatClientConfig())
        {
        }

        public GfycatClient(string clientId, string clientSecret, GfycatClientConfig config)
        {
            Config = config;
            RestClient = Config.RestClient;
            Authentication = new AuthenticationContainer(clientId, clientSecret, this);

            Debug.WriteLine($"Client created with ID \"{clientId}\"");
        }

        internal async Task<TResult> SendAsync<TResult>(string method, string endpoint, RequestOptions options = null)
        {
            return (await SendAsync(method, endpoint, options)).ReadAsJson<TResult>();
        }

        internal Task<RestResponse> SendAsync(string method, string endpoint, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            return SendInternalAsync(() => RestClient.SendAsync(method, endpoint, options.CancellationToken), options);
        }

        internal async Task<TResult> SendJsonAsync<TResult>(string method, string endpoint, object jsonObject, RequestOptions options = null)
        {
            return (await SendJsonAsync(method, endpoint, jsonObject, options)).ReadAsJson<TResult>();
        }

        internal Task<RestResponse> SendJsonAsync(string method, string endpoint, object jsonObject, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            string jsonString = JsonConvert.SerializeObject(jsonObject);
            return SendInternalAsync(() => RestClient.SendAsync(method, endpoint, jsonString, options.CancellationToken), options);
        }

        internal Task<RestResponse> SendStreamAsync(string method, string endpoint, string key, Stream stream, string fileName, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            Dictionary<string, object> content = new Dictionary<string, object>
            {
                { key, new MultipartFile(stream, fileName) }
            };
            return SendInternalAsync(() => RestClient.SendAsync(method, endpoint, content, options.CancellationToken), options);
        }
        
        private async Task<RestResponse> SendInternalAsync(Func<Task<RestResponse>> RestFunction, RequestOptions options)
        {
            RestResponse response = null;
            RestClient.SetHeader("Authentication", options.UseAccessToken ? $"Bearer {Authentication.AccessToken}" : null);
            bool retry = true, first401 = true;
            do
            {
                Task<RestResponse> taskResponse = RestFunction();
                bool taskTimedout = !taskResponse.Wait(options.Timeout ?? -1);
                if (taskTimedout && options.RetryMode != RetryMode.RetryTimeouts) // the block call will wait until the task is done, so in the following checks we can just access the result normally
                    throw new TimeoutException($"The request timed out");
                else if (taskResponse.Result.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502))
                    throw GfycatException.CreateFromResponse(taskResponse.Result);
                else if (taskResponse.Result.Status == HttpStatusCode.Unauthorized)
                    if (first401 && options.RetryMode.HasFlag(RetryMode.RetryFirst401))
                    {
                        await Authentication.AttemptRefreshTokenAsync();
                        first401 = false;
                    }
                    else
                        throw GfycatException.CreateFromResponse(taskResponse.Result);
                else
                {
                    response = taskResponse.Result;
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code != response.Status) ?? false))
                    {
                        throw GfycatException.CreateFromResponse(response);
                    }
                    retry = false;
                }
            }
            while (retry);

            return response;
        }

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username, RequestOptions options = null)
        {
            options = options ?? RequestOptions.CreateFromDefaults(Config);
            return (await SendAsync("HEAD", $"users/{username}", options)).Status == HttpStatusCode.NotFound;
        }

        public async Task SendPasswordResetEmailAsync(string usernameOrEmail)
        {
            await SendJsonAsync("PATCH", "users", new ActionRequest() { Value = usernameOrEmail, Action = "send_password_reset_email" });
        }

        public Task<User> GetUserAsync(string userId, RequestOptions options = null)
        {
            return SendAsync<User>("GET", $"users/{userId}");
        }

        public Task<CurrentUser> GetCurrentUserAsync(RequestOptions options = null)
        {
            return SendAsync<CurrentUser>("GET", "me");
        }

        public Task CreateAccountAsync(string username, string password, string email, Provider? provider = null, TokenType? authCodeType = null, string authCode = null, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(username) && (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email)))
                throw new ArgumentException($"The {nameof(username)} and {nameof(password)} or {nameof(email)} fields must not be null or empty if  is false");

            return CreateAccountInternalAsync(username, password, email, provider, authCodeType, authCode, options);
        }

        private Task CreateAccountInternalAsync(string username, string password, string email, Provider? provider, TokenType? authCodeType, string authCode, RequestOptions options = null)
        {
            JObject json = JObject.FromObject(new { username, password, email, provider });
            if (authCodeType != null)
                json.Add(_tokensToString[authCodeType.Value], authCode);
            return SendJsonAsync("POST", "users", json, options);
        }

        #endregion

        public async Task<Gfy> GetGfyAsync(string gfycat, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();
            return (await SendAsync("GET", $"gfycats/{gfycat}", options)).ReadAsJson<Gfy>();
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
            parameters = parameters ?? new GfyCreationParameters();
            parameters.FetchUrl = remoteUrl;
            return (await SendJsonAsync<GfyKey>("POST", "gfycats", parameters, options)).Gfycat;
        }

        public async Task<GfyStatus> CheckGfyUploadStatusAsync(string gfycat, RequestOptions options = null)
        {
            return await SendAsync<GfyStatus>("GET", $"gfycats/fetch/status/{gfycat}");
        }

        public async Task<string> CreateGfyAsync(Stream data, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            GfyKey uploadKey = await SendJsonAsync<GfyKey>("POST", "gfycats", parameters ?? new object(), options);
            await SendStreamAsync("POST", "https://filedrop.gfycat.com/", uploadKey.Secret, data, uploadKey.Gfycat, options);

            return uploadKey.Gfycat;
        }

        #endregion

        #region Trending feeds

        public Task<TrendingGfycatFeed> GetTrendingGfycatsAsync(string tag = null, int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>
            {
                { "tagName", tag },
                { "count", count },
                { "cursor", cursor }
            });
            return SendAsync<TrendingGfycatFeed>("GET", $"gfycats/trending{queryString}", options);
        }

        public Task<IEnumerable<string>> GetTrendingTagsAsync(int? tagCount = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>
            {
                { "tagCount", tagCount },
                { "cursor", cursor }
            });
            return SendAsync<IEnumerable<string>>("GET", $"tags/trending{queryString}", options);
        }

        public Task<GfycatFeed> GetTrendingTagsPopulatedAsync(int? tagCount = null, int? gfyCount = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>
            {
                { "tagCount", tagCount },
                { "gfyCount", gfyCount },
                { "cursor", cursor }
            });
            return SendAsync<GfycatFeed>("GET", $"tags/trending/populated{queryString}", options);
        }

        #endregion

        // supposedly in testing. hhhhhhhhhhhhhhhhmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
        public Task<GfycatFeed> SearchAsync(string searchText, int count = 50, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>
            {
                { "search_text", searchText },
                { "count", count },
                { "cursor", cursor }
            });
            return SendAsync<GfycatFeed>("GET", $"gfycats/search{queryString}", options);
        }

        #region Extras
        
        private static readonly IReadOnlyDictionary<TokenType, string> _tokensToString = new Dictionary<TokenType, string>()
        {
            { TokenType.FacebookAuthCode, "auth_code" },
            { TokenType.FacebookAccessToken, "access_token" },
            { TokenType.TwitterOauthToken, "oauth_token" },
            { TokenType.TwitterOauthTokenSecret, "oauth_token_secret" },
            { TokenType.TwitterSecret, "secret" },
            { TokenType.TwitterToken, "token" },
            { TokenType.TwitterVerifier, "verifier" }
        };

        #endregion

    }
}
