using Newtonsoft.Json;
using System.IO;
using Gfycat.Rest;

namespace Gfycat
{
    internal static class RestResponseExtensions
    {
        internal static T ReadAsJson<T>(this RestResponse response)
        {
            string responseContentAsString = response.ReadAsString();
            T resultObject = JsonConvert.DeserializeObject<T>(responseContentAsString);
            return resultObject;
        }

        internal static string ReadAsString(this RestResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Content))
                return reader.ReadToEnd();
        }
    }
}
