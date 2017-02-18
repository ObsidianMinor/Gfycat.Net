using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public class CurrentUser : User
    {
        [JsonProperty("geoWhitelist")]
        string GeoWhitelist { get; set; }
        [JsonProperty("domainWhitelist")]
        string DomainWhitelist { get; set; }
        [JsonProperty("email")]
        string Email { get; set; }

        #region Users

        public async Task<bool> GetEmailVerifiedAsync()
        {
            HttpStatusCode code = await Web.SendRequestForStatusAsync("GET", "me/email-verified", throwIf401: true);
            return code != HttpStatusCode.NotFound;
        }

        public async Task SendVerificationEmailAsync()
        {
            await Web.SendRequestAsync("POST", "me/send_verification_email");
        }

        public async Task<string> UploadProfilePictureAsync(Stream profilePic, CancellationToken? cancel = null)
        {
            string ticketResponse = await Web.SendRequestForStringAsync("POST", "me/profile_image_url");
            string ticket = ticketResponse.Split('/').Last();
            await Web.SendStreamAsync("PUT", $"https://imageupload.gfycat.com/{ticket}", profilePic, null, throwIf401:true, cancelToken:cancel);
            return ticket;
        }

        public async Task<ProfileImageUploadStatus> GetProfilePictureUploadStatusAsync(string ticket)
        {
            return (ProfileImageUploadStatus)Enum.Parse(typeof(ProfileImageUploadStatus), (await Web.SendRequestForStringAsync("GET", $"me/profile_image_url/status/{ticket}")), true);
        }

        public async Task ModifyCurrentUserAsync(params GfycatOperation[] operations)
        {
            await Web.SendJsonAsync("PATCH", "me", new { operations });
        }

        public Task FollowUserAsync(string userId)
        {
            return Web.SendRequestAsync("PUT", $"me/follows/{userId}");
        }

        public Task UnfollowUserAsync(string userId)
        {
            return Web.SendRequestAsync("DELETE", $"me/follows/{userId}");
        }

        /// <summary>
        /// Checks if this user is following the provided user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if the current user is following the provided user, otherwise false</returns>
        public async Task<bool> GetFollowingUserAsync(string userId)
        {
            return (await Web.SendRequestForStatusAsync("HEAD", $"me/follows/{userId}")) == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns an enumerable of users the current user is following
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetFollowingUsersAsync()
        {
            return Web.SendRequestAsync<IEnumerable<string>>("GET", "me/follows");
        }

        /// <summary>
        /// Returns an enumerable of users following the current user
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetUsersFollowingAsync()
        {
            return Web.SendRequestAsync<IEnumerable<string>>("GET", "me/following");
        }

        #endregion

        #region User feeds

        new public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null)
        {
            string queryString = ExtendedHttpClient.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/gfycats{queryString}");
        }

        public Task<GfycatFeed> GetTimelineFeedOfFollowingUsers(int? count = null, string cursor = null)
        {
            string queryString = ExtendedHttpClient.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/follows/gfycats{queryString}");
        }

        #endregion

        new public async Task<GfycatAlbum> GetAlbumContentsAsync(string albumId)
        {
            string endpoint = $"me/albums/{albumId}";
            return await Web.SendRequestAsync<GfycatAlbum>("GET", endpoint);
        }

        public Task<GfycatFeed> SearchAsync(string searchText, int? count = null, string cursor = null)
        {
            string queryString = ExtendedHttpClient.CreateQueryString(new Dictionary<string, object>
            {
                { "search_text", searchText },
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"me/gfycats/search{queryString}");
        }
    }
}
