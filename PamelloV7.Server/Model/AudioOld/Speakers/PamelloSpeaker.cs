using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;

namespace PamelloV7.Server.Model.AudioOld.Speakers
{
    public abstract class PamelloSpeaker : IPamelloSpeaker, IDisposable, IAsyncDisposable, IAudioModuleWithInputs<AudioPushPoint>
    {
        public IPamelloPlayerOld Player { get; }

        public bool IsDeleted { get; protected set; }
        public abstract bool IsActive { get; }

        public event Action<IPamelloSpeaker>? OnTerminated;

        public int Id { get; private set; }
        public abstract string Name { get; set; }
        
        public bool IsChangesGoing => throw new NotImplementedException();
        public void StartChanges() {
            throw new NotImplementedException();
        }

        public void EndChanges() {
            throw new NotImplementedException();
        }

        private static int _idCounter = 1;
        public PamelloSpeaker(IPamelloPlayerOld player) {
            Player = player;

            IsDeleted = false;

            Id = _idCounter++;
        }

        protected void InvokeOnDisposed() {
            OnTerminated?.Invoke(this);
        }

        public abstract string ToDiscordString();
        public abstract IPamelloDTO GetDto();
        public void Init() {
        }

        public abstract void Dispose();
        public abstract ValueTask DisposeAsync();

        public abstract AudioModel ParentModel { get; }
        public abstract bool IsDisposed { get; protected set; }
        public abstract void InitModule();

        public int MinInputs => 1;
        public int MaxInputs => 1;
        public abstract AudioPushPoint CreateInput();
    }
}
