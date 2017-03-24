using System.Threading.Tasks;

namespace Gfycat
{
    public interface ISearchable
    {
        Task<GfyFeed> SearchAsync(string searchText, RequestOptions options = null);
    }
}
