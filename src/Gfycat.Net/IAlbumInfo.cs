using System.Collections.Generic;

namespace Gfycat
{
    /// <summary>
    /// Defines an album's basic information
    /// </summary>
    public interface IAlbumInfo : IFolderInfo
    {
        /// <summary>
        /// Gets all folders inside this folder
        /// </summary>
        new IReadOnlyCollection<IAlbumInfo> Subfolders { get; }
        /// <summary>
        /// Gets whether this album is published or not
        /// </summary>
        bool Published { get; }
    }
}
