using PamelloV7.Framework.Audio.Time;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueCurrentSongRewind : PamelloCommand
{
    public Task Execute(AudioTime time) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RewindCurrent(time, ScopeUser);
    }
}

