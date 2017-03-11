using Gfycat.API;
using Gfycat.API.Models;
using Gfycat.OAuth2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : ISearchable
    {
        internal GfycatApiClient ApiClient { get; }

        public GfycatClient(string clientId, string clientSecret) : this(clientId, clientSecret, new GfycatClientConfig())
        {
        }

        public GfycatClient(string clientId, string clientSecret, GfycatClientConfig config)
        {
            ApiClient = new GfycatApiClient(clientId, clientSecret, config);

            Debug.WriteLine($"Client created with ID \"{clientId}\"");
        }

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username, RequestOptions options = null)
        {
            return (await ApiClient.GetUsernameStatusAsync(username, options)) == HttpStatusCode.NotFound;
        }

        public async Task SendPasswordResetEmailAsync(string usernameOrEmail, RequestOptions options = null)
        {
            await ApiClient.SendPasswordResetEmailAsync(new ActionRequest() { Action = "send_password_reset_email", Value = usernameOrEmail }, options);
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

        public Task CreateAccountAsync(string username, string password, string email, Provider? provider = null, TokenType? authCodeType = null, string authCode = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        private Task CreateAccountInternalAsync(string username, string password, string email, Provider? provider, TokenType? authCodeType, string authCode, RequestOptions options = null)
        {
            throw new NotImplementedException();
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
            return new GfyStatus(status);
        }

        public async Task<string> CreateGfyAsync(Stream data, GfyCreationParameters parameters = null, RequestOptions options = null)
        {
            UploadKey uploadKey = await SendJsonAsync<UploadKey>("POST", "gfycats", parameters ?? new object(), options);
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
            Feed feed = await ApiClient.SearchSiteAsync(searchText, count, cursor, options);
            return GfycatFeed.Create(this, feed);
        }

        #region API Credentials

        /// <summary>
        /// Fetches the developer keys for the current user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AppApiInfo>> GetApiCredentialsAsync(RequestOptions options = null)
        {
            return await SendAsync<IEnumerable<AppApiInfo>>("GET", "me/api-credentials", options);
        }

        #endregion

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
