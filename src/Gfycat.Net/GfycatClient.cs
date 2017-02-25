using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : IDisposable
    {
        const string _startEndpoint = "https://api.gfycat.com/v1/";
        Client _web;
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

        public AuthenticationContainer Authentication { get; }

        public GfycatClient(string clientId, string clientSecret, HttpM)
        {
            _web = new Client(invoker) { BaseAddress = new Uri(_startEndpoint) };
            Authentication = new AuthenticationContainer(clientId, clientSecret) { Client = _web };
            _web.Auth = Authentication;

            Debug.WriteLine($"Client created with ID \"{clientId}\"");
        }

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username)
        {
            return await _web.SendRequestForStatusAsync("HEAD", $"users/{username}", throwIf401:true) == HttpStatusCode.NotFound;
        }

        public async Task SendPasswordResetEmailAsync(string usernameOrEmail
        {
            await _web.SendJsonAsync("PATCH", "users", new ActionRequest() { Value = usernameOrEmail, Action = "send_password_reset_email" });
        }

        public async Task<User> GetUserAsync(string userId)
        {
            return await _web.SendRequestAsync<User>("GET", $"users/{userId}");
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            return await _web.SendRequestAsync<CurrentUser>("GET", "me");
        }
        
        public Task CreateAccountAsync(string username, string password, string email, Provider? provider = null, TokenType? authCodeType = null, string authCode = null)
        {
            if (string.IsNullOrWhiteSpace(username) && (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email)))
                throw new ArgumentException($"The {nameof(username)} and {nameof(password)} or {nameof(email)} fields must not be null or empty if  is false");

            return CreateAccountInternalAsync(username, password, email, provider, authCodeType, authCode);
        }

        private Task CreateAccountInternalAsync(string username, string password, string email, Provider? provider, TokenType? authCodeType, string authCode)
        {
            JObject json = JObject.FromObject(new { username, password, email, provider });
            if (authCodeType != null)
                json.Add(_tokensToString[authCodeType.Value], authCode);
            return _web.SendJsonAsync("POST", "users", json);
        }

        #endregion
        
        public async Task<Gfy> GetGfyAsync(string gfycat)
        {
            return await _web.SendRequestAsync<Gfy>("GET", $"gfycats/{gfycat}");
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
            return (await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters)).Gfycat;
        }

        public async Task<GfyStatus> CheckGfyUploadStatusAsync(string gfycat)
        {
            return await _web.SendRequestAsync<GfyStatus>("GET", $"gfycats/fetch/status/{gfycat}");
        }

        public async Task<string> CreateGfycatAsync(Stream data, GfyCreationParameters parameters = null, CancellationToken? cancellationToken = null)
        {
            GfyKey uploadKey = await _web.SendJsonAsync<GfyKey>("POST", "gfycats", parameters ?? new object());
            await _web.SendStreamAsync("POST", "https://filedrop.gfycat.com/", data, uploadKey.Gfycat, cancelToken: cancellationToken);
            if (cancellationToken?.IsCancellationRequested ?? false)
                return null;

            return uploadKey.Gfycat;
        }

        #endregion

        #region Trending feeds

        public Task<TrendingGfycatFeed> GetTrendingGfycatsAsync(string tag = null, int? count = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>
            {
                { "tagName", tag },
                { "count", count },
                { "cursor", cursor }
            });
            return _web.SendRequestAsync<TrendingGfycatFeed>("GET", $"gfycats/trending{queryString}");
        }

        public Task<IEnumerable<string>> GetTrendingTagsAsync(int? tagCount = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>
            {
                { "tagCount", tagCount },
                { "cursor", cursor }
            });
            return _web.SendRequestAsync<IEnumerable<string>>("GET", $"tags/trending{queryString}");
        }

        public Task<GfycatFeed> GetTrendingTagsPopulatedAsync(int? tagCount = null, int? gfyCount = null, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>
            {
                { "tagCount", tagCount },
                { "gfyCount", gfyCount },
                { "cursor", cursor }
            });
            return _web.SendRequestAsync<GfycatFeed>("GET", $"tags/trending/populated{queryString}");
        }

        #endregion

        // supposedly in testing. hhhhhhhhhhhhhhhhmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
        public Task<GfycatFeed> SearchAsync(string searchText, int count = 50, string cursor = null)
        {
            string queryString = Client.CreateQueryString(new Dictionary<string, object>
            {
                { "search_text", searchText },
                { "count", count },
                { "cursor", cursor }
            });
            return _web.SendRequestAsync<GfycatFeed>("GET", $"gfycats/search{queryString}");
        }

        public void Dispose()
        {
            ((IDisposable)_web).Dispose();
        }
    }
}
