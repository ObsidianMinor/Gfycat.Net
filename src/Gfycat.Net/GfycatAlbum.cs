using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Gfycat
{
    public class GfycatAlbum : GfycatFolderBase
    {
        protected override string InternalFolderTypeName => "albums";

        internal IUser Owner { get; set; }
        [JsonProperty("linkText")]
        public string LinkText { get; private set; }
        [JsonProperty("nsfw")]
        public NsfwSetting Nsfw { get; private set; }
        [JsonProperty("published"), JsonConverter(typeof(NumericalBooleanConverter))]
        public bool Published { get; private set; }
        [JsonProperty("order")]
        public int Order { get; private set; }
        [JsonProperty("coverImageUrl")]
        public string CoverImageUrl { get; private set; }
        [JsonProperty("coverImageUrl-mobile")]
        public string CoverImageMobileUrl { get; private set; }

        public async override Task UpdateAsync()
        {
            string requestUrl = (Owner.Id == Web.Auth.ResourceOwner) ? $"me/albums/{Id}" : $"user/{Owner.Id}/albums/{Id}";
            JsonConvert.PopulateObject(await Web.SendRequestForStringAsync("GET", requestUrl), this);
        }

        public async Task ModifyDescriptionAsync(string newDescription)
        {
            await Web.SendJsonAsync("PUT", $"me/albums/{Id}/description", new { value = newDescription });
            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting newSetting)
        {
            await Web.SendJsonAsync("PUT", $"me/albums/{Id}/nsfw", new { value = (int)newSetting });
            await UpdateAsync();
        }

        public async Task ModifyPublishSettingAsync(bool published)
        {
            await Web.SendJsonAsync("PUT", $"me/albums/{Id}/published", new { value = (published) ? "1" : "0" });
            await UpdateAsync();
        }

        public async Task ModifyOrderOfGfysAsync(IEnumerable<Gfy> gfysInNewOrder)
        {
            await Web.SendJsonAsync("PUT", $"me/albums/{Id}/order", new { newOrder = gfysInNewOrder.Select(g => g.Id) });
            await UpdateAsync();
        }

        public async Task RemoveGfysAsync(IEnumerable<Gfy> gfysToRemove)
        {
            await Web.SendJsonAsync("PATCH", $"me/albums/{Id}", new GfyFolderAction()
            {
                Action = "remove_contents",
                GfycatIds = gfysToRemove.Select(g => g.Id)
            });
            await UpdateAsync();
        }

        public async Task AddGfysAsync(IEnumerable<Gfy> gfysToAdd)
        {
            await Web.SendJsonAsync("PATCH", $"me/albums/{Id}", new GfyFolderAction()
            {
                Action = "add_to_album",
                GfycatIds = gfysToAdd.Select(g => g.Id)
            });
            await UpdateAsync();
        }
    }
}
