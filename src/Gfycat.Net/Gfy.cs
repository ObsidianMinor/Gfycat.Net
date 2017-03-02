using Gfycat.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    public class Gfy : Entity
    {
        [JsonProperty("gfyId")]
        public string Id { get; private set; }
        [JsonProperty("gfyNumber")]
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
        public long CreationDate { get; private set; }
        [JsonProperty("nsfw")]
        public NsfwSetting Nsfw { get; private set; }
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; private set; }
        [JsonProperty("likes")]
        public int Likes { get; private set; }
        [JsonProperty("published", ItemConverterType = typeof(NumericalBooleanConverter))]
        public bool Published { get; private set; } = true;
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
        [JsonProperty("gfyName")]
        public string Name { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("languageText")]
        public string LanguageText { get; private set; }
        [JsonProperty("languageCategories")]
        public IEnumerable<string> LanguageCategories { get; private set; }
        [JsonProperty("subreddit")]
        public string Subreddit { get; private set; }
        [JsonProperty("redditId")]
        public string RedditId { get; private set; }
        [JsonProperty("redditIdText")]
        public string RedditIdText { get; private set; }
        [JsonProperty("domainWhitelist")]
        public List<string> DomainWhitelist { get; private set; }
        
        public Task ShareOnTwitterAsyncAsync(string postStatus, RequestOptions options = null)
        {
            return Client.SendJsonAsync("POST", $"gfycats/{Id}/share/twitter", new { status = postStatus }, options);
        }

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/title", new { value = newTitle }, options);
            await UpdateAsync();
        }
        
        public async Task DeleteTitleAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/gfycats/{Id}/title", options);

            await UpdateAsync();
        }

        public async Task UpdateTagsAsync(IEnumerable<string> tags, RequestOptions options = null)
        {
            if (tags.Count() > 20)
                throw new ArgumentException("The number of tags provided exceeds the max value 20");

            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/tags", new { value = tags }, options);

            await UpdateAsync();
        }

        public Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", $"me/gfycats/{Id}/domain-whitelist", options);
        }

        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/domain-whitelist", new { value = newWhitelist }, options);

            await UpdateAsync();
        }

        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/gfycats/{Id}/domain-whitelist", options);

            await UpdateAsync();
        }

        public Task<IEnumerable<string>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return Client.SendAsync<IEnumerable<string>>("GET", $"me/gfycats/{Id}/geo-whitelist", options);
        }

        public async Task ModifyGeoWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/geo-whitelist", new { value = newWhitelist }, options);

            await UpdateAsync();
        }

        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/gfycats/{Id}/geo-whitelist", options);

            await UpdateAsync();
        }

        public async Task ModifyDescriptionAsync(string newDescription, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/description", new { value = newDescription }, options);

            await UpdateAsync();
        }

        public async Task DeleteDescriptionAsync(RequestOptions options = null)
        {
            await Client.SendAsync("DELETE", $"me/gfycats/{Id}/description", options);

            await UpdateAsync();
        }

        public async Task ModifyPublishedAsync(bool published, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/published", new { value = (published) ? "1" : "0" }, options);

            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting setting, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/nsfw", new { value = (int)setting }, options);

            await UpdateAsync();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            return Client.SendAsync("DELETE", $"me/gfycats/{Id}", options);
        }

        /// <summary>
        /// Returns a boolean that says whether or not the current Gfy is or isn't bookmarked
        /// </summary>
        /// <returns>True if bookmarked, false otherwise</returns>
        public async Task<bool> GetBookmarkStatusAsync(RequestOptions options = null)
        {
            return (await Client.SendAsync<dynamic>("GET", $"me/bookmarks/{Id}", options)).bookmarkState == "1";
        }

        public Task BookmarkAsync(GfycatBookmarkFolder folder = null, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(folder?.Id))
                return Client.SendAsync("PUT", $"me/bookmarks/{Id}", options);
            else
                return Client.SendAsync("PUT", $"me/bookmark-folders/{folder.Id}/contents/{Id}", options);
        }

        public Task UnbookmarkAsync(GfycatBookmarkFolder folder = null, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(folder?.Id))
                return Client.SendAsync("DELETE", $"me/bookmarks/{Id}", options);
            else
                return Client.SendAsync("DELETE", $"me/bookmark-folders/{folder.Id}/contents/{Id}", options);
        }

        public bool CurrentUserOwnsGfy() => Username == Client.Authentication.ResourceOwner;

        private async Task UpdateAsync(RequestOptions options = null)
        {
            Rest.RestResponse response = await Client.SendAsync("GET", $"gfycats/{Id}", options);
            JsonConvert.PopulateObject(response.ReadAsString(), this);
        }
    }
}
