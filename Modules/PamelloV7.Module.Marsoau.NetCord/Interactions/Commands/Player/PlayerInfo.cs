using System.Text;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player info", "Get information about the player")]
public partial class PlayerInfo
{
    public async Task Execute() {
        await RespondAsync(() =>
            Builder<ContainerBuilder>().Build()
        , () => [ScopeUser, SelectedPlayer, Queue?.CurrentSong]);
    }
    
    public class ContainerBuilder : DiscordComponentBuilder
    {
        private string GetEnabledDisabled(bool enabled) {
            return enabled
                ? DiscordString.Bold(DiscordString.Code("Enabled"))
                : DiscordString.Code("Disabled");
        }

        public ComponentContainerProperties Build() {
            var container = new ComponentContainerProperties();

            if (SelectedPlayer is null) {
                return container.AddComponents(
                    new TextDisplayProperties("No player selected")
                );
            }
            if (SelectedPlayer.Queue is null) {
                return container.AddComponents(
                    new TextDisplayProperties("Selected player has no queue")
                );
            }

            container.AddComponents(
                new TextDisplayProperties($"## {SelectedPlayer.ToDiscordString()}")
            );

            var currentEntry = SelectedPlayer.Queue.Entries.ElementAtOrDefault(SelectedPlayer.Queue.Position);
            if (currentEntry is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        new ComponentSectionThumbnailProperties(
                            new ComponentMediaProperties(SelectedPlayer.Queue.CurrentSong!.CoverUrl)
                        )
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"""
                             ### {SelectedPlayer.Queue.CurrentSong.Name}
                             {(currentEntry.Adder is not null ? $"Added by {currentEntry.Adder.ToDiscordString()}" : "Added automatically")}
                             """
                        )
                    ),
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        Button(SelectedPlayer.IsPaused ? "Resume" : "Pause", ButtonStyle.Secondary, () => {
                            Command<Framework.Commands.PlayerPauseToggle>().Execute();
                        })
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"""
                             ### {DiscordString.Code(SelectedPlayer.Queue.CurrentSongTimePosition.ToShortString())} {
                                 DiscordString.Progress((double)SelectedPlayer.Queue.CurrentSongTimePosition.TotalSeconds / SelectedPlayer.Queue.CurrentSongTimeTotal.TotalSeconds, 20)
                             } {DiscordString.Code(SelectedPlayer.Queue.CurrentSongTimeTotal.ToShortString())}
                             """
                        )
                    )
                );
            }
            else {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties("-# No current song")
                );
            }

            var currentEpisode = SelectedPlayer.Queue.CurrentEpisode;

            if (currentEpisode is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        Button("Next Episode", ButtonStyle.Secondary, () => {
                            Command<PlayerQueueGoToEpisode>().Execute("next");
                        }).WithDisabled((SelectedPlayer.Queue.CurrentSong?.Episodes.Count ?? 0) == 0)
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"{DiscordString.Code(SelectedPlayer.Queue.EpisodePosition + 1)} : {DiscordString.Code(currentEpisode.Start.ToShortString())} - {currentEpisode.Name}"
                        )
                    )
                );
            }

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Toggle", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsRandomToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"Random: {GetEnabledDisabled(SelectedPlayer.Queue.IsRandom)}")
                ),
                new ComponentSectionProperties(
                    Button("Toggle", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsReversedToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"Reversed: {GetEnabledDisabled(SelectedPlayer.Queue.IsReversed)}")
                ),
                new ComponentSectionProperties(
                    Button("Toggle", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsNoLeftoversToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"No Leftovers: {GetEnabledDisabled(SelectedPlayer.Queue.IsNoLeftovers)}")
                ),
                new ComponentSectionProperties(
                    Button("Toggle", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsFeedRandomToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"Feed Random: {GetEnabledDisabled(SelectedPlayer.Queue.IsFeedRandom)}")
                )
            );

            var speakersBuilder = new StringBuilder();

            /*
            foreach (var speaker in SelectedPlayer.ConnectedSpeakers) {
                speakersBuilder.AppendLine($"- {speaker switch {
                    IPamelloInternetSpeaker internetSpeaker => internetSpeaker.ToDiscordString(),
                    PamelloDiscordSpeaker discordSpeaker => discordSpeaker.ToDiscordString(),
                    _ => speaker.ToDiscordString()
                }}");

                var unknown = 0;
                foreach (var listener in speaker.Listeners) {
                    string discordString;

                    if (listener.User is null) {
                        if (listener is PamelloDiscordSpeakerListener discordListener) {
                            discordString = DiscordString.User(discordListener.DiscordUser.Id);
                        }
                        else {
                            unknown++;
                            continue;
                        }
                    }
                    else {
                        discordString = listener.User.ToDiscordString();
                    }

                    speakersBuilder.AppendLine($"  - {discordString}");
                }

                if (unknown > 0) speakersBuilder.AppendLine($"  - {DiscordString.Italic($"{unknown} Unknown")}");
            }
            */

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    Button("Skip", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueSkip>().Execute();
                    }).WithDisabled(SelectedPlayer.Queue.Entries.Count == 0),
                    Button("Clear Queue", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueClear>().Execute();
                    }).WithDisabled(SelectedPlayer.Queue.Entries.Count == 0)
                )
            );

            if (speakersBuilder.Length > 0) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties(speakersBuilder.ToString())
                );
            }

            return container;
        }
    }
}
