using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatFolder : ConnectedEntity
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("gfyCount")]
        public int GfyCount { get; private set; }
        [JsonProperty("publishedGfys")]
        public IEnumerable<Gfy> PublishedGfys { get; private set; }

        public async Task UpdateAsync()
        {
            JsonConvert.PopulateObject(await Web.SendRequestForStringAsync("GET", $"me/folders/{Id}"), this);
        }

        /// <summary>
        /// Deletes this folder (must be empty)
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task DeleteAsync()
        {
            return Web.SendRequestAsync("DELETE", $"me/folders/{Id}");
        }

        /// <summary>
        /// Changes the name of this folder to a new value
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task ModifyTitleAsync(string newTitle)
        {
            await Web.SendJsonAsync("PUT", $"me/folders/{Id}/name", new { value = newTitle });
            await UpdateAsync();
        }

        public async Task MoveFolderAsync(string parentFolderId)
        {

        }
    }
}
