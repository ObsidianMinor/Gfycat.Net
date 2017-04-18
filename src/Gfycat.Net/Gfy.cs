#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gfycat.API.Models;
using Model = Gfycat.API.Models.Gfy;

namespace Gfycat
{
    /// <summary>
    /// An object representation of a short, looped, soundless video moment
    /// </summary>
    [DebuggerDisplay("{Name} : {Title}")]
    public class Gfy : Entity, IUpdatable
    {
        bool _isFull;

        internal Gfy(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            FullGfy fullModel = model as FullGfy;
            _isFull = fullModel != null;

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
            UserTags = _isFull ? fullModel.UserTags?.ToReadOnlyCollection() : Enumerable.Empty<string>().ToReadOnlyCollection();
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
            LikedByCurrentUser = _isFull ? fullModel.LikeState == LikeState.Liked : false;
            DislikedByCurrentUser = _isFull ? fullModel.LikeState == LikeState.Disliked : false;
            BookmarkedByCurrentUser = _isFull ? fullModel.BookmarkState : false;
        }

        internal static Gfy Create(GfycatClient client, Model model)
        {
            Gfy returnedGfy = new Gfy(client, model.Id);
            returnedGfy.Update(model);
            return returnedGfy;
        }

        /// <summary>
        /// Gets the share page URL for this Gfy
        /// </summary>
        public string Url => $"https://gfycat.com/{Name}";

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
        /// Gets the reversed mp4 url for this gfy
        /// </summary>
        public string ReverseMp4Url => Mp4Url.Insert(Mp4Url.LastIndexOf(".mp4"), "-reverse");
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
        /// <summary>
        /// Gets the size of the mp4 version of this gfy in megabytes
        /// </summary>
        public int Mp4Size { get; private set; }
        /// <summary>
        /// Gets the size of the webm version of this gfy in megabytes
        /// </summary>
        public int WebmSize { get; private set; }
        /// <summary>
        /// Gets the size of the gif version of this gfy in megabytes
        /// </summary>
        public int GifSize { get; private set; }
        public string Source { get; private set; }
        /// <summary>
        /// Gets the creation date of this gfy
        /// </summary>
        public DateTime CreationDate { get; private set; }
        /// <summary>
        /// Gets the current nsfw setting of this gfy
        /// </summary>
        public NsfwSetting Nsfw { get; private set; }
        /// <summary>
        /// Gets the mp4 url of this gfy
        /// </summary>
        public string Mp4Url { get; private set; }
        /// <summary>
        /// Gets the number of likes for this gfy
        /// </summary>
        public int Likes { get; private set; }
        /// <summary>
        /// Gets whether this gfy is publicly available
        /// </summary>
        public bool Published { get; private set; }
        /// <summary>
        /// Gets the number of dislikes for this gfy
        /// </summary>
        public int Dislikes { get; private set; }
        /// <summary>
        /// Gets the md5 hash of this gfy
        /// </summary>
        public string Md5 { get; private set; }
        /// <summary>
        /// Gets the number of view for this gfy
        /// </summary>
        public int Views { get; private set; }
        /// <summary>
        /// Gets the tags of this gfy
        /// </summary>
        public IReadOnlyCollection<string> Tags { get; private set; }
        /// <summary>
        /// Gets the user defined tags for this gfy
        /// </summary>
        public IReadOnlyCollection<string> UserTags { get; private set; }
        /// <summary>
        /// Gets the username of the owner of this gfy
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// Gets the randomly generated URL name for this gfy
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the title of this gfy
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets the description of this gfy
        /// </summary>
        public string Description { get; private set; }
        public string LanguageText { get; private set; }
        public IEnumerable<string> LanguageCategories { get; private set; }
        /// <summary>
        /// Gets the subreddit this gfy came from
        /// </summary>
        public string Subreddit { get; private set; }
        public string RedditId { get; private set; }
        public string RedditIdText { get; private set; }
        /// <summary>
        /// Gets the whitelist of domains allowed to embed this gfy
        /// </summary>
        public IReadOnlyCollection<string> DomainWhitelist { get; private set; }
        /// <summary>
        /// Gets whether this gfy is liked by the current user
        /// </summary>
        public bool LikedByCurrentUser { get; private set; }
        /// <summary>
        /// Gets whether this gfy is disliked by the current user
        /// </summary>
        public bool DislikedByCurrentUser { get; private set; }
        /// <summary>
        /// Gets whether this gfy is bookmarked by the current user
        /// </summary>
        public bool BookmarkedByCurrentUser { get; private set; }
        
        /// <summary>
        /// Shares this gfy on twitter using the specified post status
        /// </summary>
        /// <param name="postStatus"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ShareOnTwitterAsync(string postStatus, RequestOptions options = null)
        {
            await Client.ApiClient.ShareGfyAsync(Id, new API.TwitterShareRequest() { Status = postStatus }, options);
        }

        /// <summary>
        /// Shares this gfy on twitter using the specified post status with an account verifier and token
        /// </summary>
        /// <param name="postStatus"></param>
        /// <param name="verifier"></param>
        /// <param name="token"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ShareOnTwitterAsync(string postStatus, string verifier, string token, RequestOptions options = null)
        {
            await Client.ApiClient.ShareGfyAsync(Id, new API.TwitterShareRequest() { Status = postStatus, Verifier = verifier, Token = token }, options);
        }

        /// <summary>
        /// Modifies the title of this gfy
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyTitleAsync(Id, newTitle, options);
            await UpdateAsync();
        }
        
        /// <summary>
        /// Deletes the title of this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteTitleAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyTitleAsync(Id, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Edits the tags of this gfy. Tag counts over twenty throw argument exceptions
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTagsAsync(IEnumerable<string> tags, RequestOptions options = null)
        {
            if (tags.Count() > 20)
                throw new ArgumentException("The number of tags provided exceeds the max value 20");

            await Client.ApiClient.ModifyGfyTagsAsync(Id, tags, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Gets the whitelist of domains allowed to embed this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDomainWhitelistAsync(RequestOptions options = null)
        {
            return await Client.ApiClient.GetGfyDomainWhitelistAsync(Id, options);
        }
        /// <summary>
        /// Changes the whitelist of domains allowed to embed this gfy to the new list
        /// </summary>
        /// <param name="newWhitelist"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyDomainWhitelistAsync(IEnumerable<string> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyDomainWhitelistAsync(Id, newWhitelist, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Deletes the whitelist of domains allowed to embed this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteDomainWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyDomainWhitelistAsync(Id, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Gets the whitelist of regions allowed to embed this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RegionInfo>> GetGeoWhitelistAsync(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetGfyGeoWhitelistAsync(Id, options)).Select(s => new RegionInfo(s));
        }
        /// <summary>
        /// Changes the whitelist of regions allowed to embed this gfy to the new list
        /// </summary>
        /// <param name="newWhitelist"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyGeoWhitelistAsync(IEnumerable<RegionInfo> newWhitelist, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyGeoWhitelistAsync(Id, newWhitelist.Select(r => r.TwoLetterISORegionName), options);
            await UpdateAsync();
        }
        /// <summary>
        /// Deletes the whitelist of regions allowed to embed this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteGeoWhitelistAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyGeoWhitelistAsync(Id, options);
            await UpdateAsync();
        }
        /// <summary>
        /// Modifies the description of this gfy to the new value
        /// </summary>
        /// <param name="newDescription"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyDescriptionAsync(string newDescription, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyDescriptionAsync(Id, newDescription, options);
            await UpdateAsync();
        }
        /// <summary>
        /// Deletes the description of this gfy
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteDescriptionAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteGfyDescriptionAsync(Id, options);
            await UpdateAsync();
        }
        /// <summary>
        /// Modifies the published setting of this gfy to the new value
        /// </summary>
        /// <param name="published"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyPublishSettingAsync(bool published, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyPublishedSettingAsync(Id, published, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Modifies the NSFW setting of this gfy
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyNsfwSettingAsync(NsfwSetting setting, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyGfyNsfwSettingAsync(Id, setting, options);
            await UpdateAsync();
        }
        /// <summary>
        /// Deletes this gfy on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Bookmarks this gfy using the specified bookmark folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task BookmarkAsync(BookmarkFolder folder = null, RequestOptions options = null)
        {
            await Client.ApiClient.BookmarkGfyAsync(Id, folder?.Id, options);
            await UpdateAsync();
            await folder?.UpdateAsync();
        }
        /// <summary>
        /// Unbookmarks this gfy from the specified bookmark folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UnbookmarkAsync(BookmarkFolder folder = null, RequestOptions options = null)
        {
            await Client.ApiClient.UnbookmarkGfyAsync(Id, folder?.Id, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Updates this gfy's info using the latest data from the server
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UpdateAsync(RequestOptions options = null)
        {
            Update(_isFull ? (await Client.ApiClient.GetFullGfyAsync(Id, options)).Gfy : (await Client.ApiClient.GetGfyAsync(Id, options)).GfyItem);
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
        /// <summary>
        /// Changes whether the specified gfy is liked by the current user to the specified boolean
        /// </summary>
        /// <param name="liked"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyLikedSettingAsync(bool liked, RequestOptions options = null)
        {
            if (liked)
                await Client.ApiClient.LikeGfyAsync(Id, options);
            else
                await Client.ApiClient.RemoveLikeGfyAsync(Id, options);

            await UpdateAsync();
        }
        /// <summary>
        /// Changes whether the specified gfy is disliked by the current user to the specified boolean
        /// </summary>
        /// <param name="disliked"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyDislikedSettingAsync(bool disliked, RequestOptions options = null)
        {
            if (disliked)
                await Client.ApiClient.DislikeGfyAsync(Id, options);
            else
                await Client.ApiClient.RemoveDislikeGfyAsync(Id, options);

            await UpdateAsync();
        }
    }
}
