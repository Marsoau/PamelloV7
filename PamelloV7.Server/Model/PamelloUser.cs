﻿using Discord.WebSocket;
using PamelloV7.DAL.Entity;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Modules;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.DAL;
using Microsoft.EntityFrameworkCore;
using PamelloV7.Server.Model.Difference;

namespace PamelloV7.Server.Model
{
    public class PamelloUser : PamelloEntity<DatabaseUser>
    {
        private readonly DiscordClientService _clients;
        private readonly PamelloSpeakerService _speakers;
        private readonly PamelloPlayerRepository _players;

        public SocketUser? DiscordUser { get; private set; }
        public readonly PamelloCommandsModule Commands;

        private ulong _discordId;
        private Guid _token;
        private DateTime _joinedAt;
        private int _songsPlayed;
        private bool _isAdministrator;

        public ulong DiscordId {
            get => _discordId;
        }
        public Guid Token {
            get => _token;
        }
        public DateTime JoinedAt {
            get => _joinedAt;
        }

        public int SongsPlayed {
            get => _songsPlayed;
            set {
                if (_songsPlayed == value) return;

                _songsPlayed = value;
                Save();

                _events.Broadcast(new UserSongsPlayedUpdated() {
                    UserId = Id,
                    SongsPlayed = SongsPlayed
                });
            }
        }

        public override string Name {
            get => DiscordUser?.GlobalName ?? "Undefined";
            set => throw new Exception("Unable to change user name");
        }

        public bool IsAdministrator {
            get => _isAdministrator;
            set {
                if (_isAdministrator == value) return;

                _isAdministrator = value;
                Save();

                _events.Broadcast(new UserIsAdministratorUpdated() {
                    UserId = Id,
                    IsAdministrator = IsAdministrator
                });
            }
        }

        public List<PamelloSong> _addedSongs;
        public List<PamelloPlaylist> _addedPlaylists;
        public List<PamelloSong> _favoriteSongs;
        public List<PamelloPlaylist> _favoritePlaylists;

        public IReadOnlyList<PamelloSong> AddedSongs {
            get => _addedSongs;
        }
        public IReadOnlyList<PamelloPlaylist> AddedPlaylists {
            get => _addedPlaylists;
        }
        public IReadOnlyList<PamelloSong> FavoriteSongs {
            get => _favoriteSongs;
        }
        public IReadOnlyList<PamelloPlaylist> FavoritePlaylists {
            get => _favoritePlaylists;
        }

        public IEnumerable<int> AddedSongsIds {
            get => _addedSongs.Select(entity => entity.Id);
        }
        public IEnumerable<int> AddedPlaylistsIds {
            get => _addedPlaylists.Select(entity => entity.Id);
        }
        public IEnumerable<int> FavoriteSongsIds {
            get => _favoriteSongs.Select(entity => entity.Id);
        }
        public IEnumerable<int> FavoritePlaylistsIds {
            get => _favoritePlaylists.Select(entity => entity.Id);
        }

        private PamelloPlayer? _previousPlayer;
        private PamelloPlayer? _selectedPlayer;
        public PamelloPlayer? SelectedPlayer {
            get => _selectedPlayer;
            set {
                if (_selectedPlayer == value) return;

                if (value is null && _selectedPlayer is not null) {
                    _previousPlayer = _selectedPlayer;
                }

                _selectedPlayer = value;

                _events.Broadcast(new UserSelectedPlayerIdUpdated() {
                    UserId = Id,
                    SelectedPlayerId = SelectedPlayer?.Id,
                });
            }
        }
        public PamelloPlayer RequiredSelectedPlayer {
            get => EnsurePlayerExist();
        }

        public PamelloUser(IServiceProvider services,
            DatabaseUser databaseUser
        ) : base(databaseUser, services) {
            Commands = new PamelloCommandsModule(services, this);

            _clients = services.GetRequiredService<DiscordClientService>();
            _speakers = services.GetRequiredService<PamelloSpeakerService>();
            _players = services.GetRequiredService<PamelloPlayerRepository>();

            _token = databaseUser.Token;
            _discordId = databaseUser.DiscordId;
            _joinedAt = databaseUser.JoinedAt;
            _songsPlayed = databaseUser.SongsPlayed;
            _isAdministrator = databaseUser.IsAdministrator;
        }

        protected override void InitBase() {
            if (DatabaseEntity is null) return;

            DiscordUser = _clients.GetDiscordUser(DatabaseEntity.DiscordId);

            _addedSongs = DatabaseEntity.AddedSongs.Select(e => _songs.GetRequired(e.Id)).ToList();
            _addedPlaylists = DatabaseEntity.OwnedPlaylists.Select(e => _playlists.GetRequired(e.Id)).ToList();
            _favoriteSongs = DatabaseEntity.FavoriteSongs.Select(e => _songs.GetRequired(e.Id)).ToList();
            _favoritePlaylists = DatabaseEntity.FavoritePlaylists.Select(e => _playlists.GetRequired(e.Id)).ToList();
        }

        public void TryLoadLastPlayer() {
            if (SelectedPlayer is null && _previousPlayer is not null) {
                SelectedPlayer = _previousPlayer;
            }
        }
        private PamelloPlayer EnsurePlayerExist() {
            if (SelectedPlayer is not null) return SelectedPlayer;

            PamelloPlayer? player = null;

            var vc = _clients.GetUserVoiceChannel(this);
            if (vc is not null) {
                var vcPlayers = _speakers.GetVoicePlayers(vc.Id);

                if (vcPlayers.Count == 1) {
                    player = vcPlayers.First();
                }
                if (vcPlayers.Count > 1) {
                    throw new PamelloException("Cant automaticly select a player");
                }
            }

            if (player is null) {
                player = Commands.PlayerCreate("Player").Result;
            }

            SelectedPlayer = player;
            return player;
        }

        public void AddFavoriteSong(PamelloSong song) {
            if (_favoriteSongs.Contains(song)) return;

            _favoriteSongs.Add(song);
            Save();

            song.MakeFavorited(this);
            _events.Broadcast(new UserFavoriteSongsUpdated() {
                UserId = Id,
                FavoriteSongsIds = FavoriteSongsIds,
            });
        }
        public void RemoveFavoriteSong(PamelloSong song) {
            if (!_favoriteSongs.Contains(song)) return;

            _favoriteSongs.Remove(song);
            Save();

            song.UnmakeFavorited(this);
            _events.Broadcast(new UserFavoriteSongsUpdated() {
                UserId = Id,
                FavoriteSongsIds = FavoriteSongsIds,
            });
        }

        public void AddFavoritePlaylist(PamelloPlaylist playlist) {
            if (_favoritePlaylists.Contains(playlist)) return;

            _favoritePlaylists.Add(playlist);
            Save();

            playlist.MakeFavorited(this);
            _events.Broadcast(new UserFavoritePlaylistsUpdated() {
                UserId = Id,
                FavoritePlaylistsIds = FavoritePlaylistsIds,
            });
        }
        public void RemoveFavoritePlaylist(PamelloPlaylist playlist) {
            if (!_favoritePlaylists.Contains(playlist)) return;

            _favoritePlaylists.Remove(playlist);
            Save();

            playlist.UnmakeFavorited(this);
            _events.Broadcast(new UserFavoritePlaylistsUpdated() {
                UserId = Id,
                FavoritePlaylistsIds = FavoritePlaylistsIds,
            });
            _events.Broadcast(new PlaylistFavoriteByIdsUpdated() {
                PlaylistId = playlist.Id,
                FavoriteByIds = playlist.FavoriteByIds
            });
        }

        public PamelloPlaylist CreatePlaylist(string name) {
            if (name.Length == 0) throw new PamelloException("Playlist name cant be empty");

            var db = GetDatabase();

            var dbUser = db.Users.Find(Id);
            if (dbUser is null) throw new PamelloException("User doesnt exist in the database for some reason, cant add playlist");

            var databasePlaylist = new DatabasePlaylist() {
                Name = name,
                Songs = [],
                Owner = dbUser,
                IsProtected = false,
                FavoritedBy = [],
            };

            db.Playlists.Add(databasePlaylist);
            db.SaveChanges();

            var playlist = _playlists.Load(databasePlaylist);
            playlist.Init();

            _addedPlaylists.Add(playlist);

            _events.Broadcast(new PlaylistCreated() {
                PlaylistId = Id,
            });
            _events.Broadcast(new UserAddedPlaylistsUpdated() {
                UserId = Id,
                AddedPlaylistsIds = AddedPlaylistsIds
            });

            return playlist;
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloUserDTO() {
                Id = Id,
                Name = Name,
                DiscordId = DiscordId,
                AvatarUrl = DiscordUser?.GetAvatarUrl(),
                SelectedPlayerId = SelectedPlayer?.Id,
                SongsPlayed = SongsPlayed,
                JoinedAt = JoinedAt,

                AddedSongsIds = AddedSongsIds,
                AddedPlaylistsIds = AddedPlaylistsIds,
                FavoriteSongsIds = FavoriteSongsIds,
                FavoritePlaylistsIds = FavoritePlaylistsIds,

                IsAdministrator = IsAdministrator,
            };
        }

        public override DatabaseUser GetDatabaseEntity(DatabaseContext? db = null) {
            db ??= GetDatabase();

            var dbUser = db.Users
                .Where(databaseUser => databaseUser.Id == Id)
                .Include(databaseUser => databaseUser.FavoriteSongs)
                .Include(databaseUser => databaseUser.FavoritePlaylists)
                .Include(databaseUser => databaseUser.AddedSongs)
                .Include(databaseUser => databaseUser.OwnedPlaylists)
                .AsSplitQuery()
                .FirstOrDefault();
            if (dbUser is null) throw new PamelloDatabaseSaveException();

            dbUser.SongsPlayed = SongsPlayed;
            dbUser.IsAdministrator = IsAdministrator;

            var dbAddedSongsIds = dbUser.AddedSongs.Select(song => song.Id);
            var dbAddedPlaylistsIds = dbUser.OwnedPlaylists.Select(song => song.Id);
            var dbFavoriteSongsIds = dbUser.FavoriteSongs.Select(song => song.Id);
            var dbFavoritePlaylistsIds = dbUser.FavoritePlaylists.Select(song => song.Id);

            var addedSongsDifference = DifferenceResult<int>.From(
                dbAddedSongsIds, 
                AddedSongsIds,
                true
            );
            var addedPlaylistsDifference = DifferenceResult<int>.From(
                dbAddedPlaylistsIds, 
                AddedPlaylistsIds,
                true
            );
            var favoriteSongsDifference = DifferenceResult<int>.From(
                dbFavoriteSongsIds, 
                FavoriteSongsIds,
                true
            );
            var favoritePlaylistsDifference = DifferenceResult<int>.From(
                dbFavoritePlaylistsIds, 
                FavoritePlaylistsIds,
                true
            );

            addedSongsDifference.ExcludeMoved();
            addedPlaylistsDifference.ExcludeMoved();
            favoriteSongsDifference.ExcludeMoved();
            favoritePlaylistsDifference.ExcludeMoved();

            addedSongsDifference.Apply(dbUser.AddedSongs, songId
                => db.Songs.Find(songId)!);
            addedPlaylistsDifference.Apply(dbUser.OwnedPlaylists, playlistId
                => db.Playlists.Find(playlistId)!);
            favoriteSongsDifference.Apply(dbUser.FavoriteSongs, songId
                => db.Songs.Find(songId)!);
            favoritePlaylistsDifference.Apply(dbUser.FavoritePlaylists, playlistId
                => db.Playlists.Find(playlistId)!);

            return dbUser;
        }
    }
}
