using Discord;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class PlayerInfoBuilder : PamelloDiscordComponentBuilder
{
    private string GetEnabledDisabled(bool enabled) {
        return enabled
            ? DiscordString.Bold(DiscordString.Code("Enabled"))
            : DiscordString.Code("Disabled");
    }
    public ComponentBuilderV2 Component() {
        ContainerBuilder containerBuilder;

        var componentBuilder = new ComponentBuilderV2()
            .WithContainer(containerBuilder = new ContainerBuilder());

        if (SelectedPlayer is null) {
            containerBuilder.WithTextDisplay("No player selected");
            return componentBuilder;
        }
        if (SelectedPlayer.Queue is null) {
            containerBuilder.WithTextDisplay("Selected player has no queue");
            return componentBuilder;
        }
        
        containerBuilder.WithTextDisplay($"## {SelectedPlayer.ToDiscordString()}");

        var currentEntry = SelectedPlayer.Queue.Entries.ElementAtOrDefault(SelectedPlayer.Queue.Position);
        if (currentEntry is not null) {
            containerBuilder
                .WithSeparator()
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ThumbnailBuilder()
                        .WithMedia(new UnfurledMediaItemProperties(SelectedPlayer.Queue.CurrentSong!.CoverUrl))
                    )
                    .WithTextDisplay(
                        $"""
                         ### {SelectedPlayer.Queue.CurrentSong.Name}
                         {(currentEntry.Adder is not null ? $"Added by {currentEntry.Adder.ToDiscordString()}" : "Added automatically")}
                         """
                    )
                )
                .WithSeparator()
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId("pamello-command:PlayerPauseToggle")
                        .WithLabel(SelectedPlayer.IsPaused ? "Resume" : "Pause")
                        .WithStyle(ButtonStyle.Secondary)
                    )
                    .WithTextDisplay(
                        $"""
                        ### {DiscordString.Code(SelectedPlayer.Queue.CurrentSongTimePosition.ToShortString())} {
                            DiscordString.Progress((double)SelectedPlayer.Queue.CurrentSongTimePosition.TotalSeconds / SelectedPlayer.Queue.CurrentSongTimeTotal.TotalSeconds, 20)
                        } {DiscordString.Code(SelectedPlayer.Queue.CurrentSongTimeTotal.ToShortString())}
                        """
                        
                    )
                )
                ;
        }
        else {
            containerBuilder
                .WithSeparator()
                .WithTextDisplay("-# No current song");
        }

        var currentEpisode = SelectedPlayer.Queue.CurrentEpisode;
        
        if (currentEpisode is not null) {
            containerBuilder
                .WithSeparator()
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId("pamello-command:PlayerQueueGoToEpisode?position=next")
                        .WithLabel("Next Episode")
                        .WithDisabled((SelectedPlayer.Queue.CurrentSong?.Episodes.Count ?? 0) == 0)
                        .WithStyle(ButtonStyle.Secondary)
                    )
                    .WithTextDisplay(
                        $"{DiscordString.Code(SelectedPlayer.Queue.EpisodePosition)} : {DiscordString.Code(currentEpisode.Start.ToShortString())} - {currentEpisode.Name}"
                    )
                )
                ;
        }
        
        containerBuilder
            .WithSeparator()
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("pamello-command:PlayerQueueIsRandomToggle")
                    .WithLabel("Toggle")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithTextDisplay($"Random: {GetEnabledDisabled(SelectedPlayer.Queue.IsRandom)}")
            )
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("pamello-command:PlayerQueueIsReversedToggle")
                    .WithLabel("Toggle")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithTextDisplay($"Reversed: {GetEnabledDisabled(SelectedPlayer.Queue.IsReversed)}")
            )
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("pamello-command:PlayerQueueIsNoLeftoversToggle")
                    .WithLabel("Toggle")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithTextDisplay($"No Leftovers: {GetEnabledDisabled(SelectedPlayer.Queue.IsNoLeftovers)}")
            )
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("pamello-command:PlayerQueueIsFeedRandomToggle")
                    .WithLabel("Toggle")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithTextDisplay($"Feed Random: {GetEnabledDisabled(SelectedPlayer.Queue.IsFeedRandom)}")
            );

        containerBuilder
            .WithSeparator()
            .WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId("pamello-command:PlayerQueueSkip")
                    .WithLabel("Skip")
                    .WithDisabled(SelectedPlayer.Queue.Count == 0)
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        
        return componentBuilder;
    }
}
