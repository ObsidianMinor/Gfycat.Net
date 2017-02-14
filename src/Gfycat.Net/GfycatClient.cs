using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient : IDisposable
    {
        const string _startEndpoint = "https://api.gfycat.com/v1/";
        ExtendedHttpClient _web;
        public AuthenticationContainer Authentication { get; }

        public GfycatClient(string clientId, string clientSecret)
        {
            _web = new ExtendedHttpClient() { BaseAddress = new Uri(_startEndpoint) };
            Authentication = new AuthenticationContainer(clientId, clientSecret) { Client = _web };
            _web.Auth = Authentication;

            Debug.WriteLine($"Client created with ID \"{clientId}\"");
        }

        #region Users

        public async Task<bool> UsernameIsValidAsync(string username)
        {
            return await _web.SendRequestForStatusAsync("HEAD", $"users/{username}", throwIf401:true) == HttpStatusCode.NotFound;
        }

        public async Task SendPasswordResetEmailAsync(string usernameOrEmail)
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

        public Task CreateAccountAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User feeds

        public Task<GfycatFeed> GetUserGfycatFeedAsync(string userId, int? count = null, string cursor = null)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", $"users/{userId}/gfycats", new { count, cursor });
        }

        #endregion

        #region Albums
        
        public async Task<GfycatAlbum> GetAlbumContents(string userId, string albumId)
        {
            string endpoint = $"users/{userId}/albums/{albumId}";
            return await _web.SendRequestAsync<GfycatAlbum>("GET", endpoint);
        }

        public async Task<GfycatAlbumInfo> GetAlbumContentsByLinkText(string userId, string albumLinkText)
        {
            string endpoint = $"users/{userId}/album_links/{albumLinkText}";
            return await _web.SendRequestAsync<GfycatAlbumInfo>("GET", endpoint);
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
            if (!string.IsNullOrWhiteSpace(tag))
                tag = WebUtility.UrlEncode(tag);

            return _web.SendJsonAsync<TrendingGfycatFeed>("GET", "gfycats/trending", new { tagName = tag, count, cursor });
        }

        public Task<IEnumerable<string>> GetTrendingTagsAsync(int? tagCount = null, string cursor = null)
        {
            return _web.SendJsonAsync<IEnumerable<string>>("GET", "tags/trending", new { tagCount, cursor });
        }

        public Task<GfycatFeed> GetTrendingTagsPopulatedAsync(int? tagCount = null, int? gfyCount = null, string cursor = null)
        {
            return _web.SendJsonAsync<GfycatFeed>("GET", "tags/trending/populated", new { tagCount, gfyCount, cursor });
        }

        #endregion

        public void Dispose()
        {
            ((IDisposable)_web).Dispose();
        }
    }
}
