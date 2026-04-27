using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Strings;
using PlaylistFavoriteEditModal = PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist.PlaylistFavoriteEditModal;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public enum ESongOrPlaylist { Song, Playlist }

public static class ESongOrPlaylistExtensions
{
    public static string ToShortString(this ESongOrPlaylist songOrPlaylist) {
        return songOrPlaylist switch {
            ESongOrPlaylist.Song => "song",
            ESongOrPlaylist.Playlist => "playlist",
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist), songOrPlaylist, null)
        };
    }
}

public class FavoriteListBuilder : DiscordComponentBuilder
{
    public IHistoryRecord? ClearRecord { get; private set; }
    
    public IMessageComponentProperties?[] Build(IPamelloUser user, ESongOrPlaylist songOrPlaylist, int page, int pageSize) {
        IReadOnlyList<IPamelloEntity> items = songOrPlaylist switch {
            ESongOrPlaylist.Song => user.FavoriteSongs,
            ESongOrPlaylist.Playlist => user.FavoritePlaylists,
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist))
        };
        
        if (ClearRecord is not null && items.Count > 0) ClearRecord = null;

        var title = user == ScopeUser
            ? $"Favorite {songOrPlaylist.ToShortString()}s"
            : $"Favorite {songOrPlaylist.ToShortString()}s of {user.ToDiscordString()}";

        var totalPages = items.Count / pageSize + (items.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;

        var itemsOnPage = items.Skip(page * pageSize).Take(pageSize).ToList();

        var counter = page * pageSize + 1;
        var content = items.Count == 0
            ? $"-# _No {songOrPlaylist.ToShortString()}s_"
            : string.Join("\n", itemsOnPage.Select(item => $"`{counter++}` : {songOrPlaylist switch {
                ESongOrPlaylist.Song => ((IPamelloSong)item).ToDiscordString(),
                ESongOrPlaylist.Playlist => ((IPamelloPlaylist)item).ToDiscordString(),
                _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist), songOrPlaylist, null)
            }}"));

        var container = new ComponentContainerProperties().AddComponents(
            new TextDisplayProperties($"## {title}")
        );

        if (user == ScopeUser) {
            container.AddComponents(
                new ComponentSeparatorProperties(),
                songOrPlaylist == ESongOrPlaylist.Song
                    ? new ActionRowProperties().AddComponents(
                        ModalButton<SongFavoriteEditModal>("Edit", ButtonStyle.Secondary),
                        ClearRecord is null
                            ? Button("Clear", ButtonStyle.Secondary, async () => {
                                ClearRecord = Command<SongFavoritesClear>().Execute();
                                await Message.Refresh();
                            })
                            : Button("Revert", ButtonStyle.Secondary, () => {
                                try {
                                    Command<HistoryRecordRevert>().Execute(ClearRecord);
                                }
                                finally {
                                    ClearRecord = null;
                                }
                            })
                    )
                    : new ActionRowProperties().AddComponents(
                        ModalButton<PlaylistFavoriteEditModal>("Edit", ButtonStyle.Secondary),
                        Button("Clear", ButtonStyle.Secondary, () => {
                            Command<PlaylistFavoriteClear>().Execute();
                        })
                    )
            );
        }

        container.AddComponents(
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(content),
            new ComponentSeparatorProperties()
        );

        if (items.Count > 0) {
            container.AddComponents(
                new ComponentSectionProperties(
                    Button("Add all to queue", ButtonStyle.Primary, () => {
                        Command<PlayerQueueSongAdd>().Execute(user.FavoriteSongs);
                    }).WithDisabled(songOrPlaylist != ESongOrPlaylist.Song)
                ).AddComponents(
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
                )
            );
        }
        else {
            container.AddComponents(
                new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
            );
        }

        return [
            container,
            Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, items.Count)
        ];
    }
}
