using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    internal static class RestExtensions
    {
        internal static async Task<T> ReadAsJsonAsync<T>(this RestResponse response, GfycatClientConfig config)
        {
            string responseContentAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            if (GfycatException.ContainsError(responseContentAsString))
                throw await GfycatException.CreateFromResponseAsync(response, responseContentAsString).ConfigureAwait(false);

            T resultObject = JsonConvert.DeserializeObject<T>(responseContentAsString, config.SerializerSettings);
            return resultObject;
        }

        [DebuggerStepThrough]
        internal static async Task<string> ReadAsStringAsync(this RestResponse response)
        {
            using (response.Content)
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
