using PamelloV7.Core.DTO;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public abstract class PamelloSpeaker : IPamelloSpeaker, IDisposable, IAsyncDisposable, IAudioModuleWithInputs<AudioPushPoint>
    {
        public IPamelloPlayer Player { get; }

        public bool IsDeleted { get; protected set; }
        public abstract bool IsActive { get; }

        public event Action<IPamelloSpeaker>? OnTerminated;

        public int Id { get; private set; }
        public abstract string Name { get; set; }

        private static int _idCounter = 1;
        public PamelloSpeaker(IPamelloPlayer player) {
            Player = player;

            IsDeleted = false;

            Id = _idCounter++;
        }

        protected void InvokeOnDisposed() {
            OnTerminated?.Invoke(this);
        }

        public abstract DiscordString ToDiscordString();
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
