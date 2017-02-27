using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatAlbumInfo : GfycatFolderInfoBase<GfycatAlbum>
    {
        internal IUser Owner { get; set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("linkText")]
        public string LinkText { get; private set; }
        [JsonProperty("folderSubType")]
        public string FolderSubType { get; private set; }
        [JsonProperty("coverImageUrl")]
        public string CoverImageUrl { get; private set; }
        [JsonProperty("coverImageUrl-mobile")]
        public string CoverMobileImageUrl { get; private set; }
        [JsonProperty("width")]
        public int Width { get; private set; }
        [JsonProperty("height")]
        public int Height { get; private set; }
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; private set; }
        [JsonProperty("webmUrl")]
        public string WebmUrl { get; private set; }
        [JsonProperty("webpUrl")]
        public string WebpUrl { get; private set; }
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
        public string Max5mbGif { get; private set; }
        [JsonProperty("max2mbGif")]
        public string Max2mbGif { get; private set; }
        [JsonProperty("miniUrl")]
        public string MiniUrl { get; private set; }
        [JsonProperty("miniPosterUrl")]
        public string MiniPosterUrl { get; private set; }
        [JsonProperty("mjpgUrl")]
        public string MjpgUrl { get; private set; }
        [JsonProperty("gifUrl")]
        public string GifUrl { get; private set; }
        [JsonProperty("published"), JsonConverter(typeof(NumericalBooleanConverter))]
        public bool Published { get; private set; }
        [JsonProperty("nodes")]
        public IEnumerable<GfycatAlbumInfo> Subalbums { get; private set; }

        /// <summary>
        /// Retrieves the contents of this album
        /// </summary>
        /// <returns></returns>
        public override async Task<GfycatAlbum> GetContentsAsync()
        {
            GfycatAlbum album = (Owner.Id == Authentication.ResourceOwner) // but what do I use? username or user Id?
                ? await Web.SendRequestAsync<GfycatAlbum>("GET", $"me/albums/{Id}") 
                : await Web.SendRequestAsync<GfycatAlbum>("GET", $"users/{Owner.Id}/albums/{Id}");
            album.Owner = Owner; // if we pass the owner between albums we won't need to ask the user every time they run an album method
            return album;
        }
    }
}
