using PamelloV7.Framework.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueCurrentSongRewind
{
    public Task Execute(AudioTime time) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RewindCurrent(time, ScopeUser);
    }
}

