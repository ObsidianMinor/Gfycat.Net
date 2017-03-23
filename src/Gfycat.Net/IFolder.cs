using System.Threading.Tasks;

namespace Gfycat
{
    public interface IFolder
    {
        string Id { get; }
        string Title { get; }

        Task ModifyTitleAsync(string newTitle, RequestOptions options = null);
        Task DeleteAsync(RequestOptions options = null);
    }
}
