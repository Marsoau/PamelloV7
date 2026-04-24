using System.Text;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("queue", "List the queue")]
[DiscordCommand("player queue list", "List the queue")]
public partial class PlayerQueueList
{
    public async Task Execute() {
        await RespondPageAsync(() => (Queue?.Count ?? 0, 10), page => 
            Builder<ContainerBuilder>().Build(page, 10)
        , () => [ScopeUser, SelectedPlayer, ..Queue?.Songs ?? []]);
    }

    public class ContainerBuilder : DiscordComponentBuilder
    {
        public IMessageComponentProperties?[] Build(int page, int pageSize) {
            if (SelectedPlayer is null) return [
                BasicComponentsBuilder.Info(null, "No selected player")
            ];
            if (Queue is null) return [
                BasicComponentsBuilder.Info(null, "Selected player has no queue")
            ];

            var queue = Queue.Entries;

            var totalPages = queue.Count / pageSize + (queue.Count % pageSize > 0 ? 1 : 0);

            var position = Queue.Position;

            var pageEntries = queue.Skip(page * pageSize).Take(pageSize).ToList();

            var queueBefore = pageEntries.Take(position).ToList();
            var current = pageEntries.ElementAtOrDefault(page * pageSize + position);
            var queueAfter = pageEntries.Skip(position + 1).ToList();

            var container = new ComponentContainerProperties().AddComponents(
                new TextDisplayProperties($"## {SelectedPlayer.ToDiscordString()} Queue"),
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    ModalButton<PlayerQueueSongAddModal>("Add Songs", ButtonStyle.Primary),
                    ModalButton<PlayerQueueEditModal>("Edit", ButtonStyle.Secondary),
                    ModalButton<PlayerQueueGoToModal>("Go-To", ButtonStyle.Secondary)
                        .WithDisabled(queue.Count == 0),
                    ModalButton<PlayerQueueSetNextModal>("Set Next", ButtonStyle.Secondary)
                        .WithDisabled(queue.Count <= 1)
                )
            );

            if (GetEntriesText(queueBefore, Queue.NextPositionRequest ?? -1, page * pageSize) is { Length: > 0 } beforeText) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties(beforeText)
                );
            }

            if (current is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ComponentSectionProperties(
                        new ComponentSectionThumbnailProperties(
                            new ComponentMediaProperties(current.Song?.CoverUrl ?? "")
                        )
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"""
                             ### {DiscordString.Code(page * pageSize + queueBefore.Count + 1)} : {current?.Song?.ToDiscordString()}
                             {(current?.Adder is not null ? $"Added by {DiscordString.User(current.Adder)}" : DiscordString.Italic("Added automatically"))}
                             """
                        )
                    )
                );
            }

            if (GetEntriesText(queueAfter, Queue.NextPositionRequest ?? -1, page * pageSize + queueBefore.Count + 1) is { Length: > 0 } afterText) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties(afterText)
                );
            }

            if (queue.Count == 0) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties($"-# {DiscordString.Italic("Queue is empty")}")
                );
            }

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Clear Queue", ButtonStyle.Secondary, () => {
                        Command<PlayerQueueClear>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({queue.Count} songs)")
                )
            );

            return [
                container,
                Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, queue.Count)
            ];
        }
    }
}
