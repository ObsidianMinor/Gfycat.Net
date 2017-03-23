using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public abstract class FeedEnumerator<T> : IAsyncEnumerator<T>
    {
        internal FeedEnumerator(GfycatClient client, IFeed<T> feed, int count, RequestOptions defaultOptions)
        {
            _options = defaultOptions;
            _client = client;
            _currentFeed = feed;
            _count = count;
            _currentEnumerator = _currentFeed.Content.GetEnumerator();
        }
        internal readonly RequestOptions _options;
        internal readonly GfycatClient _client;
        internal readonly int _count;
        IFeed<T> _currentFeed;
        IEnumerator<T> _currentEnumerator;

        public T Current => _currentEnumerator.Current;

        public void Dispose()
        {
            // we don't need to do anything
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (!_currentEnumerator.MoveNext())
            {
                _currentFeed = await GetNext(_currentFeed.Cursor, _count);
                _currentEnumerator = _currentFeed.Content.GetEnumerator();
                return _currentEnumerator.MoveNext();
            }
            else
                return true;
        }

        protected abstract Task<IFeed<T>> GetNext(string cursor, int count, RequestOptions options = null);
    }
}
