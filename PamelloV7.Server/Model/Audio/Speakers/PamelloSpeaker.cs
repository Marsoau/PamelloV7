namespace PamelloV7.Server.Model.Audio.Speakers
{
    public abstract class PamelloSpeaker
    {
        public readonly PamelloPlayer Player;

        public bool IsDeleted { get; protected set; }
        public abstract bool IsActive { get; }

        public event Action<PamelloSpeaker>? OnTerminated;

        private static int _idCounter = 1;
        public int Id { get; private set; }
        public PamelloSpeaker(PamelloPlayer player) {
            Player = player;

            IsDeleted = false;

            Id = _idCounter++;
        }

        public abstract Task PlayBytesAsync(byte[] audio);

        public abstract Task Terminate();

        protected void InvokeOnTerminated() {
            OnTerminated?.Invoke(this);
        }
    }
}
