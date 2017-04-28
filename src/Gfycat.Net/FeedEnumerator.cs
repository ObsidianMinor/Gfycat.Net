using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    /// <summary>
    /// Enumerates the contents of an <see cref="IFeed{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of item to enumerate</typeparam>
    public class FeedEnumerator<T> : IAsyncEnumerator<T>
    {
        internal FeedEnumerator(GfycatClient client, IFeed<T> feed, RequestOptions defaultOptions)
        {
            _options = defaultOptions;
            _client = client;
            _currentFeed = feed;
            _currentEnumerator = _currentFeed.Content.GetEnumerator();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly GfycatClient _client;

        internal readonly RequestOptions _options;
        IFeed<T> _currentFeed;
        IEnumerator<T> _currentEnumerator;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator
        /// </summary>
        public T Current => _currentEnumerator.Current;
        
        /// <summary>
        /// Defines the number of gfys to get with each request
        /// </summary>
        public int RequestCount { get; set; } = 10;

        /// <summary>
        /// Attempts to move to the next item in the current page of content. If there is no content in the current page, it retrieves the next page and returns the result of that content's move next
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_currentEnumerator.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(_currentFeed.Cursor))
                    return false;

                RequestOptions newOptions = _options?.Clone() ?? RequestOptions.CreateFromDefaults(_client.ApiClient.Config);
                newOptions.CancellationToken = cancellationToken;

                _currentFeed = await _currentFeed.GetNextPageAsync(RequestCount, newOptions).ConfigureAwait(false);
                _currentEnumerator?.Dispose();
                _currentEnumerator = _currentFeed.Content.GetEnumerator();
                return _currentEnumerator.MoveNext();
            }

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Disposes teh current feed's content enumerator
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _currentEnumerator?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the current feed's content enumerator
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
