using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gfycat.Rest;
using System.Net;
using System.Linq;

[assembly: CLSCompliant(true)]

namespace Gfycat.Analytics
{
    /// <summary>
    /// Represents a client for communicating with the Gfycat analytics API. See https://developers.gfycat.com/analytics for info on when to use this client
    /// </summary>
    public class GfycatAnalyticsClient
    {
        internal GfycatAnalyticsClientConfig Config { get; }

        /// <summary>
        /// Gets the current user's tracking cookie
        /// </summary>
        public string CurrentUserTrackingCookie { get; private set; } = GfycatAnalyticsClientConfig.GenerateCookie();

        /// <summary>
        /// Creates a new client using the specified app name, app Id, app version, and user tracking cookie
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appId"></param>
        /// <param name="appVersion"></param>
        /// <param name="userTrackingCookie"></param>
        public GfycatAnalyticsClient(string appName, string appId, Version appVersion, string userTrackingCookie)
        {
            Config = new GfycatAnalyticsClientConfig(appName, appId, appVersion);
            CurrentUserTrackingCookie = userTrackingCookie;
        }

        /// <summary>
        /// Creates a new client using the specified config and user tracking cookie
        /// </summary>
        /// <param name="config"></param>
        /// <param name="userTrackingCookie"></param>
        public GfycatAnalyticsClient(GfycatAnalyticsClientConfig config, string userTrackingCookie)
        {
            Config = config;
            CurrentUserTrackingCookie = userTrackingCookie;
        }
        
        private async Task<RestResponse> SendInternalAsync(string method, string endpoint, RequestOptions options)
        {
            RestResponse response = null;
            bool retry = true;
            while (retry)
            {
                try
                {
                    response = await Config.RestClient.SendAsync(method, endpoint, options.CancellationToken).TimeoutAfter(options.Timeout);
                }
                catch(TimeoutException) when (options.RetryMode == RetryMode.RetryTimeouts)
                {
                    continue;
                }
                if (response.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502))
                    throw await GfycatException.CreateFromResponseAsync(response);
                else
                {
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code == response.Status) ?? false))
                    {
                        throw await GfycatException.CreateFromResponseAsync(response);
                    }
                    retry = false;
                }
            }

            return response;
        }

        #region Impressions

        private async Task<RestResponse> SendImpressionInternalAsync(Impression impression, RequestOptions options)
        {
            if (impression == null)
                throw new ArgumentNullException(nameof(impression), "The provided impression can't be null!");

            return await SendInternalAsync("POST", GfycatAnalyticsClientConfig.BaseImpressionsUrl + impression.CreateQuery(this), options);
        }

        private async Task<RestResponse> SendImpressionsBulkInternalAsync(IEnumerable<Impression> impressions, RequestOptions options)
        {
            impressions = impressions.Where(impression => impression != null);

            if (impressions.Count() == 0)
                throw new ArgumentException("You can't post an impression for nothing!");

            return await SendInternalAsync("POST", GfycatAnalyticsClientConfig.BaseImpressionsUrl + impressions.CreateQuery(this), options);
        }

        /// <summary>
        /// Sends an impression using the specified gfy, context, flow, view tag, and keyword
        /// </summary>
        /// <param name="gfy"></param>
        /// <param name="context"></param>
        /// <param name="flow"></param>
        /// <param name="viewTag"></param>
        /// <param name="keyword"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendImpressionAsync(Gfy gfy, ImpressionContext context = ImpressionContext.None, ImpressionFlow flow = ImpressionFlow.None, string viewTag = null, string keyword = null, RequestOptions options = null)
        {
            await SendImpressionAsync(new Impression
            {
                Gfycat = gfy,
                Context = context,
                Flow = flow,
                Keyword = keyword,
                ViewTag = viewTag
            }, options);
        }

        /// <summary>
        /// Sends one impression to Gfycat impressions
        /// </summary>
        /// <param name="impression"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendImpressionAsync(Impression impression, RequestOptions options = null)
        {
            await SendImpressionInternalAsync(impression, options);
        }

        /// <summary>
        /// Sends multiple impressions to Gfycat impressions
        /// </summary>
        /// <param name="impressions"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendImpressionsBulkAsync(IEnumerable<Impression> impressions, RequestOptions options = null)
        {
            await SendImpressionsBulkInternalAsync(impressions, options);
        }

        #endregion

        #region Analytics

        readonly static IReadOnlyDictionary<AnalyticsEvent, string> _eventsToString = new Dictionary<AnalyticsEvent, string>
        {
            { AnalyticsEvent.AppFirstLaunch, "first_launch" },
            { AnalyticsEvent.BookmarkVideo, "bookmark_video" },
            { AnalyticsEvent.CaptionVideos, "caption_video" },
            { AnalyticsEvent.CopyLink, "copy_link" },
            { AnalyticsEvent.CreateVideo, "create_video" },
            { AnalyticsEvent.DownloadVideo, "download_video" },
            { AnalyticsEvent.ScrollInCategories, "scroll_in_categories" },
            { AnalyticsEvent.ScrollInVideos, "scoll_in_videos" },
            { AnalyticsEvent.SearchVideo, "search_videos" },
            { AnalyticsEvent.SelectCategory, "tap_category" },
            { AnalyticsEvent.ShareGfy, "send_video" }
        };

        private async Task<RestResponse> SendAnalyticsInternalAsync(AnalyticsEvent aEvent, RequestOptions options, params (string, object)[] data)
        {
            return await SendInternalAsync("POST", GfycatAnalyticsClientConfig.BaseAnalyticsUrl + Utils.CreateQueryString(
                new(string, object)[] 
                {
                    ("event", _eventsToString[aEvent]),
                    ("app", Config.AppName),
                    ("app_id", Config.AppId),
                    ("ver", Config.AppVersion.ToString())
                }.Concat(data).ToArray()
                ), options);
        }
        /// <summary>
        /// Sends a first launch event to Gfycat analytics
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendAppFirstLaunchEventAsync(RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.AppFirstLaunch, options, ("utc", CurrentUserTrackingCookie));
        }
        /// <summary>
        /// Sends a video share event to Gfycat analytics
        /// </summary>
        /// <param name="gfy"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendShareVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.ShareGfy, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }
        /// <summary>
        /// Sends a tap category event to Gfycat analytics
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendTapCategoryEventAsync(string keyword, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("The provided keyword is invalid!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.SelectCategory, options, ("utc", CurrentUserTrackingCookie), ("keyword", keyword));
        }
        /// <summary>
        /// Sends a video creation event to Gfycat analytics
        /// </summary>
        /// <param name="videoLengthMilliseconds"></param>
        /// <param name="cameraDirection"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendCreateVideoEventAsync(int? videoLengthMilliseconds = null, CameraDirection? cameraDirection = null, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.CreateVideo, options, ("utc", CurrentUserTrackingCookie), ("length", videoLengthMilliseconds), ("context", cameraDirection == CameraDirection.None ? null : cameraDirection));
        }

        /// <summary>
        /// Sends a search videos event to Gfycat analytics
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendSearchVideosEventAsync(string keyword, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("The provided keyword is invalid!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.SearchVideo, options, ("utc", CurrentUserTrackingCookie), ("keyword", keyword));
        }

        /// <summary>
        /// Sends a scoll in categories event to Gfycat analytics
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendScrollInCategoriesEventAsync(int count, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.ScrollInCategories, options, ("utc", CurrentUserTrackingCookie), ("count", count));
        }

        /// <summary>
        /// Sends a scroll in videos event to Gfycat analytics
        /// </summary>
        /// <param name="count"></param>
        /// <param name="context"></param>
        /// <param name="keyword"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendScrollInVideosEventAsync(int count, ImpressionContext context = ImpressionContext.None, string keyword = null, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.ScrollInVideos, options, ("utc", CurrentUserTrackingCookie), ("count", count), ("context", (context == ImpressionContext.None ? null : context.ToString())), ("keyword", keyword));
        }

        /// <summary>
        /// Sends a video caption event to Gfycat analytics
        /// </summary>
        /// <param name="captionText"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendCaptionVideoEventAsync(string captionText, RequestOptions options = null)
        {
            if (captionText == null)
                throw new ArgumentNullException("The caption text is null!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.CaptionVideos, options, ("utc", CurrentUserTrackingCookie), ("text", captionText));
        }

        /// <summary>
        /// Sends a copy link event to Gfycat analytics
        /// </summary>
        /// <param name="gfy"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendCopyLinkEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.CopyLink, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        /// <summary>
        /// Sends a download video event to Gfycat analytics
        /// </summary>
        /// <param name="gfy"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendDownloadVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.DownloadVideo, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        /// <summary>
        /// Sends a bookmark video event to Gfycat analytics
        /// </summary>
        /// <param name="gfy"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SendBookmarkVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.BookmarkVideo, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        #endregion
    }
}
