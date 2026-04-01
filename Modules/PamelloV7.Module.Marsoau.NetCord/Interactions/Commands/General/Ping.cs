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
public class Ping : DiscordCommand
{
    public async Task Execute([SongsDescription] List<IPamelloSong> songs) {
        var message = await RespondAsync(() => $"Songs:\n{string.Join("\n", songs)}\n`{songs.Count}`", () => [..songs]);
    }
}
