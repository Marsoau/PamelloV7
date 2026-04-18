using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway.Voice;
using NetCord.Rest;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Commands;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("add", "Add songs to the queue")]
[DiscordCommand("player queue song-add", "Add songs to the queue | has shortcut /add")]
public partial class PlayerQueueSongAdd
{
    public async Task Execute(
        [SongsDescription] List<IPamelloSong> songs,
        [Description("position", "Position in queue where to insert songs")] string? position = null
    ) {
        if (NetCordConfig.Root.Commands.AutoConnectOnAddition) {
            if (SelectedPlayer is null || !SelectedPlayer.ConnectedSpeakers.Any()) {
                var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();

                if (!speakers.GetCurrent(ScopeUser).Any()) {
                    await WithLoadingAsync(
                        Command<SpeakerDiscordConnect>().Execute(Interaction.User.Id)
                    );
                }
            }
        }
        
        var addedSongs = Command<Framework.Commands.PlayerQueueSongAdd>().Execute(songs, position).ToList();

        var events = Services.GetRequiredService<IEventsService>();
        
        events.Subscribe<SongSourceDownloadProgressUpdated>(_ => {
            UpdatableMessage?.Refresh();
        });

        await RespondOneOrManyAsync(
            addedSongs,
            song => Builder<Builder>().Build(song),
            $"Added {DiscordString.Code(addedSongs.Count)} Songs"
        );
    }
    
    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties?[] Build(IPamelloSong song) {
            var container = new ComponentContainerProperties();
            
            var downloads = Services.GetRequiredService<IDownloadService>();
            
            var downloader = song.SelectedSource is not null && downloads.IsDownloading(song.SelectedSource)
                ? downloads.GetSongDownloader(song.SelectedSource)
                : null;
            if (downloader is { Progress: 1 }) downloader = null;
           
            container.AddComponents(
                new ComponentSectionProperties(
                    new ComponentSectionThumbnailProperties(song.CoverUrl)
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ## Song Added
                         {song.ToDiscordString()}
                         """
                    )
                )
            );

            if (downloader is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties(
                        $"{DiscordString.Code("Downloading...")} {DiscordString.Progress(downloader.Progress, 10, true)}"
                    )
                );
            }
            
            return [
                container
                //Builder<BasicButtonsBuilder>().RefreshButtonRow()
            ];
        }
    }
}
