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

    public override void InitializeComponentBuilder(Differentiator differentiator, IServiceProvider services, IPamelloUser scopeUser) {
        base.InitializeComponentBuilder(differentiator, services, scopeUser);
        
        var events = services.GetRequiredService<IEventsService>();
        
        events.Subscribe<UserFavoriteSongsUpdated>(_ => {
            IEventsService.OnRecordHere(SaveLastRecord);
        });
        events.Subscribe<UserFavoriteSongsRemoved>(_ => {
            IEventsService.OnRecordHere(SaveLastRecord);
        });
        events.Subscribe<UserFavoriteSongsAdded>(_ => {
            IEventsService.OnRecordHere(SaveLastRecord);
        });
        
        return;
        
        void SaveLastRecord(IHistoryRecord? record) {
            LastRecord = record;
        }
    }

    public IMessageComponentProperties?[] Build(IPamelloUser user, ESongOrPlaylist songOrPlaylist, int page, int pageSize) {
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
                        { Nested.Event: UserFavoriteSongsRemoved removed } =>
                            GetRemovedTextDisplay(removed.RemovedSongs.Count()),
                        { Nested.Event: UserFavoriteSongsReplaced replaced } => 
                            GetReplacedTextDisplay(replaced),
                        _ => throw new ArgumentOutOfRangeException()
                    }
                )
            );

            TextDisplayProperties GetAddedTextDisplay(int count) {
                return new TextDisplayProperties($"-# Added {count} songs");
            }
            TextDisplayProperties GetRemovedTextDisplay(int count) {
                return new TextDisplayProperties($"-# Removed {count} songs");
            }
            TextDisplayProperties GetReplacedTextDisplay(UserFavoriteSongsReplaced replaced) {
                var addedSongsCount = replaced.AddedSongs.Count();
                var removedSongsCount = replaced.RemovedSongs.Count();

                if (addedSongsCount > 0 && removedSongsCount == 0) {
                    return GetAddedTextDisplay(addedSongsCount);
                }
                if (removedSongsCount > 0 && addedSongsCount == 0) {
                    return GetRemovedTextDisplay(removedSongsCount);
                }
                
                return new TextDisplayProperties($"-# {addedSongsCount} songs added and {removedSongsCount} removed");
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
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
                )
            );
        }
        else {
            container.AddComponents(
                new ComponentSeparatorProperties(),
                new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
            );
        }

        return [
            container,
            Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, items.Count)
        ];
    }
}
