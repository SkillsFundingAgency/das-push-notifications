using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Data.UnitTests.DatabaseMock
{
    [ExcludeFromCodeCoverage]
    public class InMemoryDbAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> innerEnumerator;
        private bool disposed = false;

        public InMemoryDbAsyncEnumerator(IEnumerator<T> enumerator)
        {
            this.innerEnumerator = enumerator;
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.innerEnumerator.MoveNext());
        }

        public T Current => this.innerEnumerator.Current;

        public void Dispose() 
        { 
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.innerEnumerator.Dispose();
                }

                this.disposed = true;
            }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(this.innerEnumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            if (!this.disposed)
            {
                this.innerEnumerator.Dispose();
                this.disposed = true;
            }

            return new ValueTask();
        }
    }
}
