﻿using PamelloV7.Core.DTO;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public abstract class PamelloSpeaker : IPamelloEntity, IDisposable, IAsyncDisposable
    {
        public readonly PamelloPlayer Player;

        public bool IsDeleted { get; protected set; }
        public abstract bool IsActive { get; }

        public event Action<PamelloSpeaker>? OnTerminated;

        public int Id { get; private set; }
        public abstract string Name { get; }

        private static int _idCounter = 1;
        public PamelloSpeaker(PamelloPlayer player) {
            Player = player;

            IsDeleted = false;

            Id = _idCounter++;
        }

        public abstract Task PlayBytesAsync(byte[] audio);

        protected void InvokeOnDisposed() {
            OnTerminated?.Invoke(this);
        }

        public abstract DiscordString ToDiscordString();
        public abstract IPamelloDTO GetDTO();
        
        public abstract void Dispose();
        public abstract ValueTask DisposeAsync();
    }
}
