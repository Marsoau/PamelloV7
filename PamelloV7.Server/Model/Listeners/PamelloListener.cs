namespace PamelloV7.Server.Model.Listeners
{
    public abstract class PamelloListener: IDisposable
    {
        protected readonly HttpResponse _response;

        public bool IsClosed { get; protected set; }

        public readonly TaskCompletionSource Completion;

        public event Action<PamelloListener>? OnClosed;

        private static int _idCounter = 1;
        public int Id { get; private set; }
        public PamelloListener(HttpResponse response) {
            _response = response;

            Completion = new TaskCompletionSource();

            Id = _idCounter++;
        }

        public abstract Task InitializeConnecion();
        protected abstract Task CloseConnectionBase();
        public async Task CloseConnection() {
            await CloseConnectionBase();

            Completion.SetResult();
            OnClosed?.Invoke(this);
        }

        public abstract void Dispose();
    }
}
