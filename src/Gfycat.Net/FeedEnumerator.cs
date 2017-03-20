using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    public abstract class FeedEnumerator<T> : IAsyncEnumerator<T>
    {
        internal FeedEnumerator(IFeed<T> feed, int count)
        {
            _currentFeed = feed;
            _count = count;
        }
        readonly int _count;
        IFeed<T> _currentFeed;
        IEnumerator<T> _currentEnumerator => _currentFeed.Content.GetEnumerator();

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
                return _currentFeed.Cursor != null;
            }
            else
                return true;
        }

        protected abstract Task<IFeed<T>> GetNext(string cursor, int count);
    }
}
