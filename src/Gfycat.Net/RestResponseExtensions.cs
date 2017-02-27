using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Gfycat
{
    internal static class RestResponseExtensions
    {
        internal static T ReadAsJson<T>(this RestResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Content, Encoding.UTF8))
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
        }
    }
}
