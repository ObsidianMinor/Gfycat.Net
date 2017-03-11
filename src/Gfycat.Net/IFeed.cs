using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFeed
    {
        IReadOnlyCollection<Gfy> Gfycats { get; }

        Task<IFeed> GetNextAsync(int count, RequestOptions options);
    }
}
