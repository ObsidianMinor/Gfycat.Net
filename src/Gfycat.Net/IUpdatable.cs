using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Represents an object which can be updated with information from gfycat
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Updates the object with the most recent data from gfycat
        /// </summary>
        /// <param name="options">The options for this request</param>
        /// <returns></returns>
        Task UpdateAsync(RequestOptions options = null);
    }
}
