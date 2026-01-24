using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Song;

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
                        .WithValue(count.ToString())
                        .WithLabel(source.PK.Key)
                        .WithEmote(clients.GetEmote(source.PK.Platform).Result)
                        .WithDefault(count++ == 0)
                    ).ToList())
                    .WithRequired(true)
                )
            );
        
        return modalBuilder.Build();
    }

    [ModalSubmission("song-reset-modal")]
    public async Task Submit(string songQuery) {
        var song = await GetSingleRequiredAsync<IPamelloSong>(songQuery);
        var platformString = GetSelectValue("modal-select");
        Console.WriteLine(platformString);
        if (!int.TryParse(platformString, out var platformIndex)) throw new PamelloException("Invalid source index key");

        var resetTask = Command<SongInfoReset>().Execute(song, platformIndex);

        await ReleaseInteractionAsync();
        
        await resetTask;
    }
}
