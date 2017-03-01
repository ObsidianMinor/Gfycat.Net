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

        public async override Task UpdateAsync(RequestOptions options = null)
        {
            string requestUrl = (Owner.Id == Client.Authentication.ResourceOwner) ? $"me/albums/{Id}" : $"user/{Owner.Id}/albums/{Id}";
            JsonConvert.PopulateObject((await Client.SendAsync("GET", requestUrl, options)).ReadAsString(), this);
        }

        public async Task ModifyDescriptionAsync(string newDescription, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/albums/{Id}/description", new { value = newDescription }, options);
            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting newSetting, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/albums/{Id}/nsfw", new { value = (int)newSetting });
            await UpdateAsync();
        }

        public async Task ModifyPublishSettingAsync(bool published, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/albums/{Id}/published", new { value = (published) ? "1" : "0" });
            await UpdateAsync();
        }

        public async Task ModifyOrderOfGfysAsync(IEnumerable<Gfy> gfysInNewOrder, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/albums/{Id}/order", new { newOrder = gfysInNewOrder.Select(g => g.Id) });
            await UpdateAsync();
        }

        public async Task RemoveGfysAsync(IEnumerable<Gfy> gfysToRemove, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PATCH", $"me/albums/{Id}", new GfyFolderAction()
            {
                Action = "remove_contents",
                GfycatIds = gfysToRemove.Select(g => g.Id)
            });
            await UpdateAsync();
        }

        public async Task AddGfysAsync(IEnumerable<Gfy> gfysToAdd, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PATCH", $"me/albums/{Id}", new GfyFolderAction()
            {
                Action = "add_to_album",
                GfycatIds = gfysToAdd.Select(g => g.Id)
            });
            await UpdateAsync();
        }
    }
}
