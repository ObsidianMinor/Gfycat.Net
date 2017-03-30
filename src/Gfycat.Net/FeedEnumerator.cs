using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Enumerates the contents of an <see cref="IFeed{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of item to enumerate</typeparam>
    public struct FeedEnumerator<T> : IAsyncEnumerator<T>
    {
        internal FeedEnumerator(GfycatClient client, IFeed<T> feed, RequestOptions defaultOptions)
        {
            _options = defaultOptions;
            _client = client;
            _currentFeed = feed;
            _currentEnumerator = _currentFeed.Content.GetEnumerator();
        }
        internal readonly RequestOptions _options;
        internal readonly GfycatClient _client;
        IFeed<T> _currentFeed;
        IEnumerator<T> _currentEnumerator;

        public T Current => _currentEnumerator.Current;
        
        public void Dispose()
        {
            // we don't need to do anything
        }

        /// <summary>
        /// Attempts to move to the next item in the current page of content. If there is no content in the current page, it retrieves the next page and returns the result of that content's move next
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (!_currentEnumerator.MoveNext())
            {
                RequestOptions newOptions = _options.Clone();
                newOptions.CancellationToken = cancellationToken;

                _currentFeed = await _currentFeed.GetNextPageAsync(newOptions);
                _currentEnumerator = _currentFeed.Content.GetEnumerator();
                return _currentEnumerator.MoveNext();
            }

            return true;
        }
    }
}
