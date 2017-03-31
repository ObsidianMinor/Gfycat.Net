using Newtonsoft.Json;
using System.IO;
using Gfycat.Rest;
using System.Diagnostics;

namespace Gfycat.Rest
{
    internal static class RestExtensions
    {
        internal static T ReadAsJson<T>(this RestResponse response, GfycatClientConfig config)
        {
            string responseContentAsString = response.ReadAsString();
            T resultObject = JsonConvert.DeserializeObject<T>(responseContentAsString, config.SerializerSettings);
            return resultObject;
        }

        [DebuggerStepThrough]
        internal static string ReadAsString(this RestResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Content))
                return reader.ReadToEnd();
        }
    }
}
