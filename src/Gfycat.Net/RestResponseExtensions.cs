using Newtonsoft.Json;
using System.IO;

namespace Gfycat
{
    internal static class RestResponseExtensions
    {
        internal static T ReadAsJson<T>(this RestResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Content))
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
        }

        internal static string ReadAsString(this RestResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Content))
                return reader.ReadToEnd();
        }
    }
}
