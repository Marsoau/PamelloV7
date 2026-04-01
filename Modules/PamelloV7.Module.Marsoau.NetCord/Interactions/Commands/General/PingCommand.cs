using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Descriptions;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
[DiscordCommand("execution clap", "clap")]
[DiscordCommand("ado film red", "red")]
public class PingCommand : DiscordCommand
{
    public async Task Execute([SongDescription] string songQuery = "aa") {
        var song = await GetSingleRequiredAsync<IPamelloSong>(songQuery);
        
        var message = await RespondAsync(() => $"Song: {song}", () => [song]);
    }
}
