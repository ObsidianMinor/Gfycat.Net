using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatBookmarkFolderInfo : GfycatFolderInfoBase<GfycatBookmarkFolder>
    {
        [JsonProperty("nodes")]
        public IEnumerable<GfycatBookmarkFolderInfo> Subfolders { get; private set; }

        public override Task<GfycatBookmarkFolder> GetContentsAsync()
        {
            return Web.SendRequestAsync<GfycatBookmarkFolder>("GET", $"me/bookmark-folders/{Id}");
        }
    }
}
