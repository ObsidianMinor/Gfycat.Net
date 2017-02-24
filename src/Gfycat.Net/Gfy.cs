using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    public class Gfy : ConnectedEntity
    {
        private static readonly UnauthorizedAccessException _invalidOwnership = new UnauthorizedAccessException("The current user doesn't own this resource");

        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("number")]
        public long Number { get; private set; }
        [JsonProperty("webmUrl")]
        public string WebmUrl { get; private set; }
        [JsonProperty("gifUrl")]
        public string GifUrl { get; private set; }
        [JsonProperty("mobileUrl")]
        public string MobileUrl { get; private set; }
        [JsonProperty("mobilePosterUrl")]
        public string MobilePosterUrl { get; private set; }
        [JsonProperty("posterUrl")]
        public string PosterUrl { get; private set; }
        [JsonProperty("thumb360Url")]
        public string Thumb360Url { get; private set; }
        [JsonProperty("thumb360PosterUrl")]
        public string Thumb360PosterUrl { get; private set; }
        [JsonProperty("thumb100PosterUrl")]
        public string Thumb100PosterUrl { get; private set; }
        [JsonProperty("max5mbGif")]
        public string Max5MbGif { get; private set; }
        [JsonProperty("max2mbGif")]
        public string Max2MbGif { get; private set; }
        [JsonProperty("max1mbGif")]
        public string Max1MbGif { get; private set; }
        [JsonProperty("mjpgUrl")]
        public string MjpgUrl { get; private set; }
        [JsonProperty("width")]
        public int Width { get; private set; }
        [JsonProperty("height")]
        public int Height { get; private set; }
        [JsonProperty("avgColor")]
        public string AverageColor { get; private set; }
        [JsonProperty("frameRate")]
        public int FrameRate { get; private set; }
        [JsonProperty("numFrames")]
        public int NumberOfFrames { get; private set; }
        [JsonProperty("mp4Size")]
        public int Mp4Size { get; private set; }
        [JsonProperty("webmSize")]
        public int WebmSize { get; private set; }
        [JsonProperty("gifSize")]
        public int GifSize { get; private set; }
        [JsonProperty("source")]
        public string Source { get; private set; }
        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; private set; }
        [JsonProperty("nsfw")]
        public bool Nsfw { get; private set; }
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; private set; }
        [JsonProperty("likes")]
        public int Likes { get; private set; }
        [JsonProperty("published"), JsonConverter(typeof(NumericalBooleanConverter))]
        public bool Published { get; private set; }
        [JsonProperty("dislikes")]
        public int Dislikes { get; private set; }
        [JsonProperty("extraLemmas")]
        public string ExtraLemmas { get; private set; }
        [JsonProperty("md5")]
        public string Md5 { get; private set; }
        [JsonProperty("views")]
        public int Views { get; private set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("languageText")]
        public string LanguageText { get; private set; }
        [JsonProperty("languageCategories")]
        public string LanguageCategories { get; private set; }
        [JsonProperty("subreddit")]
        public string Subreddit { get; private set; }
        [JsonProperty("redditId")]
        public string RedditId { get; private set; }
        [JsonProperty("redditIdText")]
        public string RedditIdText { get; private set; }
        [JsonProperty("domainWhitelist")]
        public List<string> DomainWhitelist { get; private set; }
        
        public Task ShareOnTwitterAsyncAsync(string postStatus)
        {
            return Web.SendJsonAsync("POST", $"gfycats/{Id}/share/twitter", new { status = postStatus });
        }

        public async Task ModifyTitleAsync(string newTitle)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/title", new { value = newTitle });
            await UpdateAsync();
        }
        
        public async Task DeleteTitleAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendRequestAsync("DELETE", $"me/gfycats/{Id}/title");

            await UpdateAsync();
        }

        public async Task UpdateTagsAsync(params string[] tags)
        {
            if (tags.Count() > 20)
                throw new ArgumentException("The number of tags provided exceeds the max value 20");
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/tags", new { value = tags });

            await UpdateAsync();
        }

        public Task<IEnumerable<string>> GetDomainWhitelistAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            return Web.SendRequestAsync<IEnumerable<string>>("GET", $"me/gfycats/{Id}/domain-whitelist");
        }

        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/domain-whitelist", new { value = newWhitelist });

            await UpdateAsync();
        }

        public async Task DeleteDomainWhitelistAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendRequestAsync("DELETE", $"me/gfycats/{Id}/domain-whitelist");

            await UpdateAsync();
        }

        public Task<IEnumerable<string>> GetGeoWhitelistAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            return Web.SendRequestAsync<IEnumerable<string>>("GET", $"me/gfycats/{Id}/geo-whitelist");
        }

        public async Task ModifyGeoWhitelistAsync(IEnumerable<string> newWhitelist)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/geo-whitelist", new { value = newWhitelist });

            await UpdateAsync();
        }

        public async Task DeleteGeoWhitelistAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendRequestAsync("DELETE", $"me/gfycats/{Id}/geo-whitelist");

            await UpdateAsync();
        }

        public async Task ModifyDescriptionAsync(string newDescription)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/description", new { value = newDescription });

            await UpdateAsync();
        }

        public async Task DeleteDescriptionAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendRequestAsync("DELETE", $"me/gfycats/{Id}/description");

            await UpdateAsync();
        }

        public async Task ModifyPublishedAsync(bool published)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/published", new { value = (published) ? "1" : "0" });

            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting setting)
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            await Web.SendJsonAsync("PUT", $"me/gfycats/{Id}/nsfw", new { value = (int)setting });

            await UpdateAsync();
        }

        public Task DeleteAsync()
        {
            if (!CurrentUserOwnsGfy())
                throw _invalidOwnership;

            return Web.SendRequestAsync("DELETE", $"me/gfycats/{Id}");
        }

        /// <summary>
        /// Returns a boolean that says whether or not the current Gfy is or isn't bookmarked
        /// </summary>
        /// <returns>True if bookmarked, false otherwise</returns>
        public async Task<bool> GetBookmarkStatusAsync()
        {
            return (await Web.SendRequestAsync<dynamic>("GET", $"me/bookmarks/{Id}")).bookmarkState == "1";
        }

        public Task BookmarkAsync(GfycatBookmarkFolder folder = null)
        {
            if (string.IsNullOrWhiteSpace(folder?.Id))
                return Web.SendRequestAsync("PUT", $"me/bookmarks/{Id}");
            else
                return Web.SendRequestAsync("PUT", $"me/bookmark-folders/{folder.Id}/contents/{Id}");
        }

        public Task UnbookmarkAsync(GfycatBookmarkFolder folder = null)
        {
            if (string.IsNullOrWhiteSpace(folder?.Id))
                return Web.SendRequestAsync("DELETE", $"me/bookmarks/{Id}");
            else
                return Web.SendRequestAsync("DELETE", $"me/bookmark-folders/{folder.Id}/contents/{Id}");
        }
        
        public bool CurrentUserOwnsGfy() => Web.Auth.ResourceOwner == Username;

        private async Task UpdateAsync()
        {
            string result = await Web.SendRequestForStringAsync("GET", $"gfycats/{Id}");
            JsonConvert.PopulateObject(result, this);
        }
    }
}
