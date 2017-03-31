using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gfycat.Rest;
using System.Net;
using System.Linq;

namespace Gfycat.Analytics
{
    public class GfycatAnalyticsClient
    {
        internal GfycatAnalyticsClientConfig Config { get; }

        public string CurrentUserTrackingCookie { get; private set; } = GfycatAnalyticsClientConfig.GenerateCookie();

        public GfycatAnalyticsClient(string appName, string appId, Version appVersion, string userTrackingCookie)
        {
            Config = new GfycatAnalyticsClientConfig(appName, appId, appVersion);
            CurrentUserTrackingCookie = userTrackingCookie;
        }

        public GfycatAnalyticsClient(GfycatAnalyticsClientConfig config, string userTrackingCookie)
        {
            Config = config;
            CurrentUserTrackingCookie = userTrackingCookie;
        }
        
        private Task<RestResponse> SendInternalAsync(string method, string endpoint, RequestOptions options)
        {
            RestResponse response = null;
            bool retry = true;
            while (retry)
            {
                Task<RestResponse> taskResponse = Config.RestClient.SendAsync(method, endpoint, options.CancellationToken);
                bool taskTimedout = !taskResponse.Wait(options.Timeout);
                if (taskTimedout && options.RetryMode != RetryMode.RetryTimeouts) // the blocking wait call will complete the task, so in the following checks we can just access the result normally
                    throw new TimeoutException("The request timed out");
                else if (taskResponse.Result.Status == HttpStatusCode.BadGateway && !options.RetryMode.HasFlag(RetryMode.Retry502))
                    throw GfycatException.CreateFromResponse(taskResponse.Result);
                else
                {
                    response = taskResponse.Result;
                    if (!response.Status.IsSuccessfulStatus() && !(options.IgnoreCodes?.Any(code => code == response.Status) ?? false))
                    {
                        throw GfycatException.CreateFromResponse(response);
                    }
                    retry = false;
                }
            }

            return Task.FromResult(response);
        }

        #region Impressions

        private async Task<RestResponse> SendImpressionInternalAsync(Impression impression, RequestOptions options)
        {
            if (impression == null)
                throw new ArgumentNullException(nameof(impression), "The provided impression can't be null!");

            return await SendInternalAsync("POST", GfycatAnalyticsClientConfig.BaseImpressionsUrl + impression.CreateQuery(), options);
        }

        private async Task<RestResponse> SendImpressionsBulkInternalAsync(IEnumerable<Impression> impressions, RequestOptions options)
        {
            impressions = impressions.Where(impression => impression != null);

            if (impressions.Count() == 0)
                throw new ArgumentException("You can't post an impression for nothing!");

            return await SendInternalAsync("POST", GfycatAnalyticsClientConfig.BaseImpressionsUrl + impressions.CreateQuery(), options);
        }

        public async Task SendImpressionAsync(Gfy gfy, ImpressionContext context = ImpressionContext.None, ImpressionFlow flow = ImpressionFlow.None, string viewTag = null, string keyword = null, RequestOptions options = null)
        {
            await SendImpressionAsync(new Impression
            {
                AppId = Config.AppId,
                Version = Config.AppVersion.ToString(),
                SessionTrackingCookie = Config.SessionId,
                UserTrackingCookie = CurrentUserTrackingCookie,
                GfycatId = gfy.Id,
                Context = (context == ImpressionContext.None ? null : context.ToString()),
                Flow = (flow == ImpressionFlow.None ? null : flow.ToString()),
                Keyword = keyword,
                ViewTag = viewTag
            }, options);
        }

        public async Task SendImpressionAsync(Impression impression, RequestOptions options = null)
        {
            await SendImpressionInternalAsync(impression, options);
        }

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

        public async Task SendAppFirstLaunchEventAsync(RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.AppFirstLaunch, options, ("utc", CurrentUserTrackingCookie));
        }

        public async Task SendShareVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.ShareGfy, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        public async Task SendTapCategoryEventAsync(string keyword, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("The provided keyword is invalid!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.SelectCategory, options, ("utc", CurrentUserTrackingCookie), ("keyword", keyword));
        }

        public async Task SendCreateVideoEventAsync(int? videoLengthMilliseconds = null, CameraDirection? cameraDirection = null, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.CreateVideo, options, ("utc", CurrentUserTrackingCookie), ("length", videoLengthMilliseconds), ("context", cameraDirection));
        }

        public async Task SendSearchVideosEventAsync(string keyword, RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("The provided keyword is invalid!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.SearchVideo, options, ("utc", CurrentUserTrackingCookie), ("keyword", keyword));
        }

        public async Task SendScrollInCategoriesEventAsync(int count, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.ScrollInCategories, options, ("utc", CurrentUserTrackingCookie), ("count", count));
        }

        public async Task SendScrollInVideosEventAsync(int count, ImpressionContext context = ImpressionContext.None, string keyword = null, RequestOptions options = null)
        {
            await SendAnalyticsInternalAsync(AnalyticsEvent.ScrollInVideos, options, ("utc", CurrentUserTrackingCookie), ("count", count), ("context", (context == ImpressionContext.None ? null : context.ToString())), ("keyword", keyword));
        }

        public async Task SendCaptionVideoEventAsync(string captionText, RequestOptions options = null)
        {
            if (captionText == null)
                throw new ArgumentNullException("The caption text is null!");

            await SendAnalyticsInternalAsync(AnalyticsEvent.CaptionVideos, options, ("utc", CurrentUserTrackingCookie), ("text", captionText));
        }

        public async Task SendCopyLinkEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.CopyLink, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        public async Task SendDownloadVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.DownloadVideo, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        public async Task SendBookmarkVideoEventAsync(Gfy gfy, RequestOptions options = null)
        {
            if (gfy == null)
                throw new ArgumentNullException("The provided Gfy was null");

            await SendAnalyticsInternalAsync(AnalyticsEvent.BookmarkVideo, options, ("utc", CurrentUserTrackingCookie), ("gfyid", gfy.Id));
        }

        #endregion
    }
}
