using System;
using System.Collections.Generic;
using System.Linq;

namespace Gfycat.Analytics
{
    internal static class QueryExtensions
    {
        internal static string CreateQuery(this Impression impression, GfycatAnalyticsClient client)
        {
            return Utils.CreateQueryString(
                ("app_id", client.Config.AppId), 
                ("ver", client.Config.AppVersion),
                ("utc", client.CurrentUserTrackingCookie),
                ("stc", client.Config.SessionId),
                ("gfyid", impression.Gfycat.Id),
                ("context", impression.Context == ImpressionContext.None ? null : impression.Context.ToString()),
                ("keyword", impression.Keyword),
                ("flow", impression.Flow == ImpressionFlow.None ? null : impression.Flow.ToString()),
                ("viewtag", impression.ViewTag));
        }

        internal static string CreateQuery(this IEnumerable<Impression> impressions, GfycatAnalyticsClient client)
        {
            Impression impression = impressions.FirstOrDefault();

            IEnumerable<(string, object)> queryStringEnumerable = new (string, object)[] {
                ("app_id", client.Config.AppId), 
                ("ver", client.Config.AppVersion),
                ("utc", client.CurrentUserTrackingCookie),
                ("stc", client.Config.SessionId),
                ("gfyid", impression.Gfycat.Id),
                ("context", impression.Context),
                ("keyword", impression.Keyword),
                ("flow", impression.Flow),
                ("viewtag", impression.ViewTag)
            };

            for(int i = 1; i < impressions.Count(); i++)
            {
                impression = impressions.ElementAt(i);
                queryStringEnumerable.Concat(new(string, object)[] {
                    ($"gfyid_{i}", impression.Gfycat.Id),
                    ($"context_{i}", impression.Context),
                    ($"keyword_{i}", impression.Keyword),
                    ($"flow_{i}", impression.Flow),
                    ($"viewtag_{i}", impression.ViewTag)
                });
            }

            return Utils.CreateQueryString(queryStringEnumerable.ToArray());
        }
    }
}
