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
        
        public virtual async Task UpdateAsync()
        {
            JsonConvert.PopulateObject(await Web.SendRequestForStringAsync("GET", $"me/{InternalFolderTypeName}/{Id}"), this);
        }

        /// <summary>
        /// Deletes this folder (must be empty)
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task DeleteAsync()
        {
            return Web.SendRequestAsync("DELETE", $"me/{InternalFolderTypeName}/{Id}");
        }
        
        /// <summary>
        /// Changes the name of this folder to a new value
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual async Task ModifyTitleAsync(string newTitle)
        {
            await Web.SendJsonAsync("PUT", $"me/{InternalFolderTypeName}/{Id}/name", new { value = newTitle });
            await UpdateAsync();
        }
        
        public virtual async Task MoveFolderAsync(string parentId)
        {
            await Web.SendJsonAsync("PUT", $"me/{InternalFolderTypeName}/{Id}", new { parent_id = parentId });
            await UpdateAsync();
        }

        public virtual async Task CreateFolderAsync(string title = "New Folder")
        {
            await Web.SendJsonAsync("POST", $"me/{InternalFolderTypeName}/{Id}", new { folderName = title });
        }
        
        public virtual async Task MoveGfysAsync(string parentId, params Gfy[] gfys)
        {
            await Web.SendJsonAsync("PATCH", $"me/{InternalFolderTypeName}/{Id}", new GfyFolderAction()
            {
                Action = "move_contents",
                ParentId = parentId,
                GfycatIds = gfys.Select(g => g.Id)
            });
            await UpdateAsync();
        }
    }
}