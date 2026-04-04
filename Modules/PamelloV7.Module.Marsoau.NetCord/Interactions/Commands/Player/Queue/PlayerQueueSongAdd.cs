using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway.Voice;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("ado film red", "Add songs to the queue")]
public partial class PlayerQueueSongAdd
{
    public async Task Execute(
        [SongsDescription] List<IPamelloSong> songs
    ) {
        var addedSongs = Command<Framework.Commands.PlayerQueueSongAdd>().Execute(songs);
        
        await RespondAsync(() =>
            Builder<AddedSongsBuilder>().GetForOne(addedSongs.First())
        , () => [..addedSongs]);
    }
}
