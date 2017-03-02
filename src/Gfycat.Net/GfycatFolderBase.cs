using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gfycat
{
    public abstract class GfycatFolderBase : Entity
    {
        protected abstract string InternalFolderTypeName { get; }

        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("gfyCount")]
        public int GfyCount { get; private set; }
        [JsonProperty("publishedGfys")]
        public IEnumerable<Gfy> PublishedGfys { get; private set; }
        
        public virtual async Task UpdateAsync(RequestOptions options = null)
        {
            JsonConvert.PopulateObject((await Client.SendAsync("GET", $"me/{InternalFolderTypeName}/{Id}", options)).ReadAsString(), this);
        }

        /// <summary>
        /// Deletes this folder (must be empty)
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task DeleteAsync(RequestOptions options = null)
        {
            return Client.SendAsync("DELETE", $"me/{InternalFolderTypeName}/{Id}", options);
        }
        
        /// <summary>
        /// Changes the name of this folder to a new value
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/{InternalFolderTypeName}/{Id}/name", new { value = newTitle }, options);
            await UpdateAsync();
        }
        
        public virtual async Task MoveFolderAsync(string parentId, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PUT", $"me/{InternalFolderTypeName}/{Id}", new { parent_id = parentId }, options);
            await UpdateAsync();
        }

        public virtual async Task CreateFolderAsync(string title = "New Folder", RequestOptions options = null)
        {
            await Client.SendJsonAsync("POST", $"me/{InternalFolderTypeName}/{Id}", new { folderName = title }, options);
        }
        
        public virtual async Task MoveGfysAsync(string parentId, IEnumerable<Gfy> gfys, RequestOptions options = null)
        {
            await Client.SendJsonAsync("PATCH", $"me/{InternalFolderTypeName}/{Id}", new GfyFolderAction()
            {
                Action = "move_contents",
                ParentId = parentId,
                GfycatIds = gfys.Select(g => g.Id)
            }, options);
            await UpdateAsync();
        }
    }
}