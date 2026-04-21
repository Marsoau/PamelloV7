using System.Text;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;
using PamelloV7.Module.Marsoau.NetCord.Speakers;
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
            if (Queue is null) {
                return container.AddComponents(
                    new TextDisplayProperties("Selected player has no queue")
                );
            }

            container.AddComponents(
                new TextDisplayProperties($"## {SelectedPlayer.ToDiscordString()}")
            );

            var currentEntry = Queue.Entries.ElementAtOrDefault(Queue.Position);
            if (currentEntry is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        new ComponentSectionThumbnailProperties(
                            new ComponentMediaProperties(Queue.CurrentSong!.CoverUrl)
                        )
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"""
                             ### {Queue.CurrentSong.Name}
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
                             ### {DiscordString.Code(Queue.CurrentSongTimePosition.ToShortString())} {
                                 DiscordString.Progress((double)Queue.CurrentSongTimePosition.TotalSeconds / Queue.CurrentSongTimeTotal.TotalSeconds, 20)
                             } {DiscordString.Code(Queue.CurrentSongTimeTotal.ToShortString())}
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

            var currentEpisode = Queue.CurrentEpisode;

            if (currentEpisode is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        Button("Next Episode", ButtonStyle.Secondary, () => {
                            Command<PlayerQueueGoToEpisode>().Execute("next");
                        }).WithDisabled((Queue.CurrentSong?.Episodes.Count ?? 0) == 0)
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"{DiscordString.Code(Queue.EpisodePosition + 1)} : {DiscordString.Code(currentEpisode.Start.ToShortString())} - {currentEpisode.Name}"
                        )
                    )
                );
            }

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button(Queue.IsRandom ? "Enabled" : "Disabled", Queue.IsRandom ? ButtonStyle.Primary : ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsRandomToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"### Random")
                ),
                new ComponentSectionProperties(
                    Button(Queue.IsReversed ? "Enabled" : "Disabled", Queue.IsReversed ? ButtonStyle.Primary : ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsReversedToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"### Reversed")
                ),
                new ComponentSectionProperties(
                    Button(Queue.IsNoLeftovers ? "Enabled" : "Disabled", Queue.IsNoLeftovers ? ButtonStyle.Primary : ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsNoLeftoversToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"### No Leftovers")
                ),
                new ComponentSectionProperties(
                    Button(Queue.IsFeedRandom ? "Enabled" : "Disabled", Queue.IsFeedRandom ? ButtonStyle.Primary : ButtonStyle.Secondary, () => {
                        Command<PlayerQueueIsFeedRandomToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"### Feed Random")
                )
            );

            var speakersBuilder = new StringBuilder();

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
                            discordString = DiscordString.User(discordListener.DiscordId);
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

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    ModalButton<PlayerQueueSongAddModal>("Add Songs", ButtonStyle.Primary),
                    ModalButton<PlayerQueueRewindModal>("Rewind", ButtonStyle.Secondary).WithDisabled(Queue.CurrentSong is null),
                    Button("Skip", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueSkip>().Execute();
                    }).WithDisabled(Queue.Entries.Count == 0)
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
