using System;
using System.Collections.Generic;
using System.Linq;

namespace Gfycat
{
    public static class FolderExtensions
    {
        /// <summary>
        /// Flattens a Gfycat folder tree into one enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderInfo"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this T folderInfo) where T : IFolderInfo
        {
            return folderInfo.Subfolders.Select(subfolder => (T)subfolder).SelectMany(c => c.Flatten()).Concat(folderInfo.Subfolders.Select(subfolder => (T)subfolder));
        }
    }
}
