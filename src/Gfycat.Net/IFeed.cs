using System.Collections.Generic;

namespace Gfycat
{
    public interface IFeed<T> : IAsyncEnumerable<T>
    {
        IReadOnlyCollection<T> Content { get; }
        string Cursor { get; }
    }
}
