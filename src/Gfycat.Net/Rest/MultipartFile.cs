using System.IO;

namespace Gfycat.Rest
{
    public struct MultipartFile
    {
        internal Stream Stream { get; }
        internal string FileName { get; }

        public MultipartFile(Stream stream, string fileName)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}