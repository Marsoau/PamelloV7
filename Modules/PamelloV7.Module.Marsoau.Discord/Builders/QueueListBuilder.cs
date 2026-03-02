using System.Text;
using Discord;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class QueueListBuilder : PamelloDiscordComponentBuilder
{
    public ComponentBuilderV2 Get(int page, int pageSize) {
        var container = new ContainerBuilder();
        
        if (SelectedPlayer?.Queue is null) return Builder<BasicComponentsBuilder>().Info("No queue");
        
        var queue = SelectedPlayer.Queue.Entries;

        var totalPages = queue.Count / pageSize + (queue.Count % pageSize > 0 ? 1 : 0);

        var position = SelectedPlayer.Queue.Position;
        
        var pageEntries = queue.Skip(page * pageSize).Take(pageSize);
        
        var queueBefore = pageEntries.Take(position).ToList();
        var current = pageEntries.ElementAtOrDefault(page * pageSize + position);
        var queueAfter = pageEntries.Skip(position + 1).ToList();
        
        container.WithTextDisplay($"## {SelectedPlayer.ToDiscordString()} Queue");
        container.WithSeparator();

        container.WithActionRow(new ActionRowBuilder()
            .WithButton(new ButtonBuilder()
                .WithCustomId("player-queue-add")
                .WithLabel("Add Songs")
                .WithDisabled(true)
                .WithStyle(ButtonStyle.Primary)
            )
            .WithButton(new ButtonBuilder()
                .WithCustomId("player-queue-edit")
                .WithLabel("Edit")
                .WithDisabled(true)
                .WithStyle(ButtonStyle.Secondary)
            )
            .WithButton(new ButtonBuilder()
                .WithCustomId("player-queue-goto")
                .WithLabel("Go-To")
                .WithDisabled(queue.Count == 0)
                .WithStyle(ButtonStyle.Secondary)
            )
            .WithButton(new ButtonBuilder()
                .WithCustomId("player-queue-set-next")
                .WithLabel("Set Next")
                .WithDisabled(queue.Count <= 1)
                .WithStyle(ButtonStyle.Secondary)
            )
        );
        

        if (GetEntriesText(queueBefore, SelectedPlayer.Queue.NextPositionRequest ?? -1, page * pageSize) is { Length: > 0 } beforeText) {
            container.WithSeparator();
            container.WithTextDisplay(beforeText);
        }

        if (current is not null) {
            container.WithSeparator();
            container.WithSection(new SectionBuilder()
                .WithAccessory(new ThumbnailBuilder()
                    .WithMedia(new UnfurledMediaItemProperties(current.Song?.CoverUrl))
                )
                .WithTextDisplay($"### {DiscordString.Code(page * pageSize + queueBefore.Count + 1)} : {current?.Song?.ToDiscordString()}\n{(current?.Adder is not null ? $"Added by {DiscordString.User(current.Adder)}" : DiscordString.Italic("Added automatically"))}")
            );
        }
        
        if (GetEntriesText(queueAfter, SelectedPlayer.Queue.NextPositionRequest ?? -1, page * pageSize + queueBefore.Count + 1) is { Length: > 0 } afterText) {
            container.WithSeparator();
            container.WithTextDisplay(afterText);
        }

        if (queue.Count == 0) {
            container.WithTextDisplay($"-# {DiscordString.Italic("Queue is empty")}");
        }

        container.WithSeparator();
        container.WithSection(new SectionBuilder()
            .WithAccessory(new ButtonBuilder()
                .WithCustomId("pamello-command:PlayerQueueClear")
                .WithLabel("Clear Queue")
                .WithStyle(ButtonStyle.Secondary)
            )
            .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({queue.Count} songs)")
        );

        return Builder<ButtonsBuilder>().PageButtons(
            new ComponentBuilderV2().WithContainer(container)
        , page != 0, page < totalPages - 1);
    }

    public string GetEntriesText(IEnumerable<PamelloQueueEntry> list, int nextPosition, int countStart) {
        var sb = new StringBuilder();
        
        foreach (var entry in list) {
            if (entry.Song is null) continue;
            
            sb.AppendLine($"{DiscordString.Code(countStart + 1)} {(countStart == nextPosition ? DiscordString.Italic("next >") : ":")} {entry.Song.ToDiscordString()}");
            
            countStart++;
        }
        
        return sb.ToString();
    }
}
