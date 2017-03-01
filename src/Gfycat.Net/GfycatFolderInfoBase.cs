using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Gfycat
{
    public abstract class GfycatFolderInfoBase<T> : Entity where T : GfycatFolderBase
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }

        public abstract Task<T> GetContentsAsync(RequestOptions options = null);
    }
}
