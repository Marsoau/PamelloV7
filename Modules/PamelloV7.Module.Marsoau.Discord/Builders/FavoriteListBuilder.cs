using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Enumerators;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class FavoriteListBuilder : PamelloDiscordComponentBuilder
{
    public ComponentBuilderV2 Component(IPamelloUser user, ESongOrPlaylist songOrPlaylist, int page, int pageSize) {
        IReadOnlyList<IPamelloEntity> items = songOrPlaylist switch {
            ESongOrPlaylist.Song => user.FavoriteSongs,
            ESongOrPlaylist.Playlist => user.FavoritePlaylists,
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist))
        };
        
        var title = user == ScopeUser ? $"Favorite {songOrPlaylist.ToShortString()}s" : $"Favorite {songOrPlaylist.ToShortString()}s of {user.ToDiscordString()}";

        var totalPages = items.Count / pageSize + (items.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;
        
        var itemsOnPage = items.Skip(page * pageSize).Take(pageSize).ToList();
        
        var counter = page * pageSize + 1;
        var content = items.Count == 0 ? $"-# _No {songOrPlaylist.ToShortString()}s_" :
            string.Join("\n", itemsOnPage.Select(item => $"`{counter++}` : {songOrPlaylist switch {
                ESongOrPlaylist.Song => ((IPamelloSong)item).ToDiscordString(),
                ESongOrPlaylist.Playlist => ((IPamelloPlaylist)item).ToDiscordString(),
            }}"));
        
        var containerBuilder = new ContainerBuilder();

        containerBuilder
            .WithTextDisplay($"## {title}");

        if (user == ScopeUser) {
            containerBuilder.WithSeparator();
            containerBuilder.WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-{songOrPlaylist.ToShortString()}s-edit:{user.Id}")
                    .WithLabel("Edit")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-{songOrPlaylist.ToShortString()}s-clear:{user.Id}")
                    .WithLabel("Clear")
                    .WithDisabled(items.Count == 0)
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        }

        containerBuilder
            .WithSeparator()
            .WithTextDisplay(content)
            .WithSeparator();

        if (items.Count > 0) {
            containerBuilder
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId("refresh")
                        //.WithCustomId($"favorite-list-add:{user.Id}")
                        .WithLabel("Add all to queue")
                        .WithStyle(ButtonStyle.Primary)
                    )
                    .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
                );
        }
        else {
            containerBuilder
                .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({items.Count} songs)");
        }
        
        var componentBuilder = new ComponentBuilderV2().WithContainer(containerBuilder);

        return Builder<ButtonsBuilder>().PageButtons(componentBuilder, page > 0, page < totalPages - 1);
    }
}
