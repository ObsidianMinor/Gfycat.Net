using System.Collections.Generic;

namespace Gfycat
{
    public interface IFeed<out T> : IAsyncEnumerable<T>
    {
        /// <summary>
        /// Contains the current page of content for this feed
        /// </summary>
        IReadOnlyCollection<T> Content { get; }
        /// <summary>
        /// The cursor use to get the next feed
        /// </summary>
        string Cursor { get; }
    }
}
