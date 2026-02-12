using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueCurrentSongRewind : PamelloCommand
{
    public Task Execute(AudioTime time) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RewindCurrent(time);
    }
}

