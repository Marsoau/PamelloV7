using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Song;

public class SongSourceSelectModal : DiscordModal
{
    public static Modal Build(IPamelloSong song, IServiceProvider services) {
        var clients = services.GetRequiredService<DiscordClientService>();
        
        var count = 0;
        
        var modalBuilder = new ModalBuilder()
            .WithTitle("Select source")
            .WithCustomId($"song-source-select-modal:{song.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithSelectMenu("Source", new SelectMenuBuilder()
                    .WithCustomId("modal-select")
                    .WithOptions(song.Sources.Select(authorization => new SelectMenuOptionBuilder()
                        .WithValue(count.ToString())
                        .WithLabel(authorization.PK.Key)
                        .WithEmote(clients.GetEmote(authorization.PK.Platform).Result)
                        .WithDefault(count++ == song.SelectedSourceIndex)
                    ).ToList())
                    .WithRequired(true)
                , "Source used to download/play/reset the song")
            );
        
        return modalBuilder.Build();
    }
    
    [ModalSubmission("song-source-select-modal")]
    public async Task Submit(string songQuery) {
        var song = await GetSingleRequiredAsync<IPamelloSong>(songQuery);
        var sourceString = GetSelectValue("modal-select");
        Output.Write($"sourceString: {sourceString}");
        if (!int.TryParse(sourceString, out var sourceIndex)) throw new PamelloException("Invalid authorization index key");

        Output.Write($"Changing source from {song.SelectedSourceIndex}");

        Command<SongSourceSelect>().Execute(song, sourceIndex);

        Output.Write($"Changed source to {song.SelectedSourceIndex}");
        
        await ReleaseInteractionAsync();
    }
}
