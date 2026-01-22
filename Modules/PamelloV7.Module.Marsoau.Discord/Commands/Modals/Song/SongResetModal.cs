using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song;

[Modal("song-reset-modal")]
public class SongResetModal : DiscordModal
{
    public static Modal Build(IPamelloSong song, IServiceProvider services) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var count = 0;
        
        var modalBuilder = new ModalBuilder()
            .WithTitle("Reset song info")
            .WithCustomId($"song-reset-modal:{song.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithSelectMenu("Source", new SelectMenuBuilder()
                    .WithCustomId("modal-select")
                    .WithOptions(song.Sources.Select(source => new SelectMenuOptionBuilder()
                        .WithDefault(count++ == 0)
                        .WithLabel(source.PK.Key)
                        .WithEmote(clients.GetEmote(source.PK.Platform).Result)
                        .WithValue(source.PK.ToString())
                    ).ToList())
                    .WithRequired(true)
                )
            );
        
        return modalBuilder.Build();
    }

    public override async Task Submit(string songQuery) {
        var song = _peql.GetSingleRequired<IPamelloSong>(songQuery, User);
        var platformKey = GetSelectValue("modal-select");

        Command<SongInfoReset>().Execute(song, platformKey);

        await ReleaseInteractionAsync();
    }
}
