using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    /// <summary>
    /// Represents an abstract REST client for all Gfycat requests
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Sets the value of a header key in the default request headers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetHeader(string key, string value);
        void SetCancellationToken(CancellationToken token);

        /// <summary>
        /// Sends a request to the given endpoint
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token);
        /// <summary>
        /// Sends a request with json content to the given endpoint
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token);
        /// <summary>
        /// Sends a request with stream content to the given endpoint
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="stream"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token);
        /// <summary>
        /// Sends a request with mulipart content to the given endpoint
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="multipart"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token);
    }
}
