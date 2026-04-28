using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events.Actions;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
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
    public IHistoryRecord? LastRecord { get; private set; }
    
    private bool _subscribed;

    public IMessageComponentProperties?[] Build(IPamelloUser user, ESongOrPlaylist songOrPlaylist, int page, int pageSize) {
        Output.Write("Fucking builder");
        if (!_subscribed) {
            var events = Services.GetRequiredService<IEventsService>();

            if (songOrPlaylist == ESongOrPlaylist.Song) {
                events.Subscribe<UserFavoriteSongsUpdated>(_ => IEventsService.OnRecordHere(SaveLastRecord));
            }
            else {
                events.Subscribe<UserFavoritePlaylistsUpdated>(_ => IEventsService.OnRecordHere(SaveLastRecord));
            }
            
            _subscribed = true;
            
            void SaveLastRecord(IHistoryRecord? record) {
                LastRecord = record;
            }
        }
        
        IReadOnlyList<IPamelloEntity> items = songOrPlaylist switch {
            ESongOrPlaylist.Song => user.FavoriteSongs,
            ESongOrPlaylist.Playlist => user.FavoritePlaylists,
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist))
        };

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
                        Button("Clear", ButtonStyle.Secondary, async () => {
                            Command<SongFavoritesClear>().Execute();
                        }).WithDisabled(user.FavoriteSongs.Count == 0)
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
            new TextDisplayProperties(content)
        );

        if (LastRecord is not null) {
            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Revert", ButtonStyle.Secondary, () => {
                        Command<HistoryRecordRevert>().Execute(LastRecord);
                    })
                ).AddComponents(
                    LastRecord switch {
                        { Nested.Event: UserFavoriteSongsAdded added } =>
                            GetAddedTextDisplay(added.AddedSongs.Count()),
                        { Nested.Event: UserFavoritePlaylistsAdded added } =>
                            GetAddedTextDisplay(added.AddedPlaylists.Count()),
                        { Nested.Event: UserFavoriteSongsRemoved removed } =>
                            GetRemovedTextDisplay(removed.RemovedSongs.Count()),
                        { Nested.Event: UserFavoritePlaylistsRemoved removed } =>
                            GetRemovedTextDisplay(removed.RemovedPlaylists.Count()),
                        { Nested.Event: UserFavoriteSongsReplaced replaced } => 
                            GetReplacedTextDisplay(replaced.AddedSongs.Count(), replaced.RemovedSongs.Count()),
                        { Nested.Event: UserFavoritePlaylistsReplaced replaced } => 
                            GetReplacedTextDisplay(replaced.AddedPlaylists.Count(), replaced.RemovedPlaylists.Count()),
                        _ => throw new ArgumentOutOfRangeException()
                    }
                )
            );

            TextDisplayProperties GetAddedTextDisplay(int count) {
                return new TextDisplayProperties($"-# Added {count} {songOrPlaylist.ToShortString()}s");
            }
            TextDisplayProperties GetRemovedTextDisplay(int count) {
                return new TextDisplayProperties($"-# Removed {count} {songOrPlaylist.ToShortString()}s");
            }
            TextDisplayProperties GetReplacedTextDisplay(int addedCount, int removedCount) {
                if (addedCount > 0 && removedCount == 0) {
                    return GetAddedTextDisplay(addedCount);
                }
                if (removedCount > 0 && addedCount == 0) {
                    return GetRemovedTextDisplay(removedCount);
                }
                
                return new TextDisplayProperties($"-# {addedCount} {songOrPlaylist.ToShortString()}s added and {removedCount} removed");
            }
        }

        if (items.Count > 0) {
            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Add all to queue", ButtonStyle.Primary, () => {
                        Command<PlayerQueueSongAdd>().Execute(user.FavoriteSongs);
                    }).WithDisabled(songOrPlaylist != ESongOrPlaylist.Song)
                ).AddComponents(
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} {songOrPlaylist.ToShortString()}s)")
                )
            );
        }
        else {
            container.AddComponents(
                new ComponentSeparatorProperties(),
                new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} {songOrPlaylist.ToShortString()}s)")
            );
        }

        return [
            container,
            Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, items.Count)
        ];
    }
}
