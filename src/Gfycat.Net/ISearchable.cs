using System.Threading.Tasks;

namespace Gfycat
{
    public interface ISearchable
    {
        Task<IFeed> SearchAsync(string searchText, int count, string cursor, RequestOptions options);
    }
}
