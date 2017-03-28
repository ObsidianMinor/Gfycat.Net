using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    public interface IRestClient
    {
        void SetHeader(string key, string value);
        void SetCancellationToken(CancellationToken token);

        Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token);
        Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token);
        Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token);
        Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token);
    }
}
