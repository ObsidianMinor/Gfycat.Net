using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Gfy;

namespace Gfycat
{
    public class Gfy : Entity, IUpdatable
    {
        internal Gfy(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            throw new NotImplementedException();
        }

        internal static Gfy Create(GfycatClient client, Model model)
        {
            Gfy returnedGfy = new Gfy(client, model.Id);
            returnedGfy.Update(model);
            return returnedGfy;
        }

        public long Number { get; private set; }
        public string WebmUrl { get; private set; }
        public string GifUrl { get; private set; }
        public string MobileUrl { get; private set; }
        public string MobilePosterUrl { get; private set; }
        public string PosterUrl { get; private set; }
        public string Thumb360Url { get; private set; }
        public string Thumb360PosterUrl { get; private set; }
        public string Thumb100PosterUrl { get; private set; }
        public string Max5MbGif { get; private set; }
        public string Max2MbGif { get; private set; }
        public string Max1MbGif { get; private set; }
        public string MjpgUrl { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string AverageColor { get; private set; }
        public int FrameRate { get; private set; }
        public int NumberOfFrames { get; private set; }
        public int Mp4Size { get; private set; }
        public int WebmSize { get; private set; }
        public int GifSize { get; private set; }
        public string Source { get; private set; }
        public long CreationDate { get; private set; }
        public NsfwSetting Nsfw { get; private set; }
        public string Mp4Url { get; private set; }
        public int Likes { get; private set; }
        public bool Published { get; private set; }
        public int Dislikes { get; private set; }
        public string ExtraLemmas { get; private set; }
        public string Md5 { get; private set; }
        public int Views { get; private set; }
        public IReadOnlyCollection<string> Tags { get; private set; }
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string LanguageText { get; private set; }
        public IEnumerable<string> LanguageCategories { get; private set; }
        public string Subreddit { get; private set; }
        public string RedditId { get; private set; }
        public string RedditIdText { get; private set; }
        public IReadOnlyCollection<string> DomainWhitelist { get; private set; }
        
        public async Task ShareOnTwitterAsyncAsync(string postStatus, RequestOptions options = null)
        {
            await Client.ApiClient.ShareGfyAsync(Id, new API.TwitterShareRequest() { Status = postStatus }, options);
        }

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/gfycats/{Id}/title", new { value = newTitle }, options);
            await UpdateAsync();
        }
        
        public async Task DeleteTitleAsync(RequestOptions options = null)
        {
            await ClientSendAsync("DELETE", $"me/gfycats/{Id}/title", options);

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

        public async Task UpdateAsync(RequestOptions options = null)
        {
            Model model = await Client.GetGfyAsync(Id, options);
            Update(model);
        }
    }
}
