using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public interface IRestClient
    {
        void SetHeader(string key, string value);
        void SetCancellationToken(CancellationToken token);

        Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token);
        Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token);
        Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token);
    }
}
