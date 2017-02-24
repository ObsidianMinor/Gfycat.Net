using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatFolderInfo : GfycatFolderInfoBase<GfycatFolder>
    {
        [JsonProperty("nodes")]
        public IEnumerable<GfycatFolderInfo> Subfolders { get; private set; }

        public override Task<GfycatFolder> GetContentsAsync()
        {
            return Web.SendRequestAsync<GfycatFolder>("GET", $"me/folders/{Id}");
        }
    }
}
