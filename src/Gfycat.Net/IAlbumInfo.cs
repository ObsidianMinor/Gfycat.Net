using System.Collections.Generic;

namespace Gfycat
{
    public interface IAlbumInfo : IFolderInfo
    {
        new IReadOnlyCollection<IAlbumInfo> Subfolders { get; }
        bool Published { get; }
    }
}
