using System.Threading.Tasks;

namespace Gfycat
{
    public interface IUpdatable
    {
        Task UpdateAsync(RequestOptions options = null);
    }
}
