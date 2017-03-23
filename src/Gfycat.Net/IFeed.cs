using System.Collections.Generic;

namespace Gfycat
{
    public interface IFeed<out T> : IAsyncEnumerable<T>
    {
        IReadOnlyCollection<T> Content { get; }
        string Cursor { get; }
    }
}
