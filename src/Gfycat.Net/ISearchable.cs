using System.Threading.Tasks;

namespace Gfycat
{
    public interface ISearchable
    {
        Task<GfyFeed> SearchAsync(string searchText, int count = 10, RequestOptions options = null);
    }
}
