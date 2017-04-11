using Gfycat.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class FullGfy : Gfy
    {
        [JsonProperty("likeState")]
        internal LikeState LikeState { get; set; }
        [JsonProperty("bookmarkState"), JsonConverter(typeof(NumericalBooleanConverter))]
        internal bool BookmarkState { get; set; }
        [JsonProperty("userTags")]
        internal IEnumerable<string> UserTags { get; set; }
        [JsonProperty("fullDomainWhitelist")]
        internal IEnumerable<string> FullDomainWhitelist { get; set; }
        [JsonProperty("fullGeoWhitelist")]
        internal IEnumerable<string> FullGeoWhitelist { get; set; }
    }

    internal enum LikeState
    {
        Disliked = -1,
        None,
        Liked
    }
}
