using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Gfy;

namespace Gfycat
{
    /// <summary>
    /// An object representation of a short, looped, soundless video moment
    /// </summary>
    [DebuggerDisplay("{Name} : {Title}")]
    public class Gfy : Entity, IUpdatable
    {
        internal Gfy(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            Number = model.Number;
            WebmUrl = model.WebmUrl;
            GifUrl = model.GifUrl;
            MobileUrl = model.MobileUrl;
            MobilePosterUrl = model.MobilePosterUrl;
            PosterUrl = model.PosterUrl;
            Thumb360Url = model.Thumb360Url;
            Thumb360PosterUrl = model.Thumb360PosterUrl;
            Thumb100PosterUrl = model.Thumb100PosterUrl;
            Max5MbGif = model.Max5MbGif;
            Max2MbGif = model.Max2MbGif;
            Max1MbGif = model.Max1MbGif;
            MjpgUrl = model.MjpgUrl;
            Width = model.Width;
            Height = model.Height;
            AverageColor = model.AverageColor;
            FrameRate = model.FrameRate;
            NumberOfFrames = model.NumberOfFrames;
            Mp4Size = model.Mp4Size;
            WebmSize = model.WebmSize;
            GifSize = model.GifSize;
            Source = model.Source;
            CreationDate = model.CreationDate;
            Nsfw = model.Nsfw;
            Mp4Url = model.Mp4Url;
            Likes = model.Likes;
            Published = model.Published;
            Dislikes = model.Dislikes;
            Md5 = model.Md5;
            Views = model.Views;
            Tags = model.Tags.ToReadOnlyCollection();
            Username = model.Username;
            Name = model.Name;
            Title = model.Title;
            Description = model.Description;
            LanguageText = model.LanguageText;
            LanguageCategories = model.LanguageCategories;
            Subreddit = model.Subreddit;
            RedditId = model.RedditId;
            RedditIdText = model.RedditIdText;
            DomainWhitelist = model.DomainWhitelist.ToReadOnlyCollection();
    }

        internal static Gfy Create(GfycatClient client, Model model)
        {
            Gfy returnedGfy = new Gfy(client, model.Id);
            returnedGfy.Update(model);
            return returnedGfy;
        }

        public long Number { get; private set; }
        /// <summary>
        /// Gets the webm url for this gfy
        /// </summary>
        public string WebmUrl { get; private set; }
        /// <summary>
        /// Gets the gif url for this gfy
        /// </summary>
        public string GifUrl { get; private set; }
        /// <summary>
        /// Gets the mobile url for this gfy
        /// </summary>
        public string MobileUrl { get; private set; }
        /// <summary>
        /// Gets the mobile cover image for this gfy
        /// </summary>
        public string MobilePosterUrl { get; private set; }
        /// <summary>
        /// Gets the cover image for this gfy
        /// </summary>
        public string PosterUrl { get; private set; }
        /// <summary>
        /// Gets the 360mb thumbnail url for this gfy
        /// </summary>
        public string Thumb360Url { get; private set; }
        /// <summary>
        /// Gets the 360mb thumbnail cover url for this gfy
        /// </summary>
        public string Thumb360PosterUrl { get; private set; }
        /// <summary>
        /// Gets the 100mb thumbnail cover url for this gfy
        /// </summary>
        public string Thumb100PosterUrl { get; private set; }
        /// <summary>
        /// Gets the 5mb gif url for this gfy
        /// </summary>
        public string Max5MbGif { get; private set; }
        /// <summary>
        /// Gets the 2mb gif url for this gfy
        /// </summary>
        public string Max2MbGif { get; private set; }
        /// <summary>
        /// Gets the 1mb gif url for this gfy
        /// </summary>
        public string Max1MbGif { get; private set; }
        /// <summary>
        /// Gets the mjpg url for this gfy
        /// </summary>
        public string MjpgUrl { get; private set; }
        /// <summary>
        /// Gets the width of the cover image for this gfy
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Gets the height of the cover image for this gfy
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Gets the average color for this gfy
        /// </summary>
        public string AverageColor { get; private set; }
        /// <summary>
        /// Gets the framerate for this gfy
        /// </summary>
        public int FrameRate { get; private set; }
        /// <summary>
        /// Gets the number of frames for this gfy
        /// </summary>
        public int NumberOfFrames { get; private set; }
        public int Mp4Size { get; private set; }
        public int WebmSize { get; private set; }
        public int GifSize { get; private set; }
        public string Source { get; private set; }
        public DateTime CreationDate { get; private set; }
        public NsfwSetting Nsfw { get; private set; }
        public string Mp4Url { get; private set; }
        public int Likes { get; private set; }
        public bool Published { get; private set; }
        public int Dislikes { get; private set; }
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
            await Client.ApiClient.ModifyGfyTitleAsync(Id, newTitle, options);
            await UpdateAsync();
        }
        
        public async Task DeleteTitleAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyTitleAsync(Id, options);
            await UpdateAsync();
        }

        public async Task ModifyTagsAsync(IEnumerable<string> tags, RequestOptions options = null)
        {
            if (tags.Count() > 20)
                throw new ArgumentException("The number of tags provided exceeds the max value 20");

            await Client.ApiClient.ModifyGfyTagsAsync(Id, tags, options);
            await UpdateAsync();
        }

        public async Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return await Client.ApiClient.GetGfyDomainWhitelistAsync(Id, options);
        }

        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyDomainWhitelistAsync(Id, newWhitelist, options);
            await UpdateAsync();
        }

        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyDomainWhitelistAsync(Id, options);
            await UpdateAsync();
        }

        public async Task<IEnumerable<RegionInfo>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetGfyGeoWhitelistAsync(Id, options)).Select(s => new RegionInfo(s));
        }

        public async Task ModifyGeoWhitelistAsync(IEnumerable<RegionInfo> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyGeoWhitelistAsync(Id, newWhitelist.Select(r => r.TwoLetterISORegionName), options);
            await UpdateAsync();
        }

        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyGeoWhitelistAsync(Id, options);
            await UpdateAsync();
        }

        public async Task ModifyDescriptionAsync(string newDescription, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyDescriptionAsync(Id, newDescription, options);
            await UpdateAsync();
        }

        public async Task DeleteDescriptionAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyDescriptionAsync(Id, options);
            await UpdateAsync();
        }

        public async Task ModifyPublishSettingAsync(bool published, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyPublishedSettingAsync(Id, published, options);
            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting setting, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyNsfwSettingAsync(Id, setting, options);
            await UpdateAsync();
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyAsync(Id, options);
        }

        /// <summary>
        /// Returns a boolean that says whether or not the current Gfy is or isn't bookmarked
        /// </summary>
        /// <returns>True if bookmarked, false otherwise</returns>
        public async Task<bool> GetBookmarkStatusAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetBookmarkedStatusAsync(Id, options)).BookmarkStatus;
        }

        public async Task BookmarkAsync(BookmarkFolder folder = null, RequestOptions options = null)
        {
            await Client.ApiClient.BookmarkGfyAsync(Id, folder?.Id, options);
        }

        public async Task UnbookmarkAsync(BookmarkFolder folder = null, RequestOptions options = null)
        {
            await Client.ApiClient.UnbookmarkGfyAsync(Id, folder?.Id, options);
        }

        public async Task UpdateAsync(RequestOptions options = null)
        {
            Update((await Client.ApiClient.GetGfyAsync(Id, options)).GfyItem);
        }

        /// <summary>
        /// Returns the creator of this <see cref="Gfy"/>. If it was uploaded or created anonymously, this returns null
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(RequestOptions options = null)
        {
            return (Username != "anonymous") ? await Client.GetUserAsync(Username, options) : null;
        }
    }
}
