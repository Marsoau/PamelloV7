using PamelloV7.Core.AudioOld;

namespace PamelloV7.Server.Model.Listeners
{
    public abstract class PamelloListener : IPamelloListener, IDisposable
    {
        protected readonly HttpResponse _response;

        public bool IsClosed { get; protected set; }

        public TaskCompletionSource Completion { get; }

        public event Action<PamelloListener>? OnClosed;

        private static int _idCounter = 1;
        public int Id { get; }
        public PamelloListener(HttpResponse response) {
            _response = response;

            Completion = new TaskCompletionSource();

            Id = _idCounter++;
        }

        protected abstract Task CloseConnectionBase();
        public async Task CloseConnection() {
            await CloseConnectionBase();

            Completion.SetResult();
            OnClosed?.Invoke(this);
        }

        public abstract void Dispose();
        public abstract Task InitializeConnection();
    }
}
