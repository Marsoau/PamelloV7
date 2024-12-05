using Discord.WebSocket;
using PamelloV7.DAL.Entity;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Modules;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model
{
    public class PamelloUser : PamelloEntity<DatabaseUser>
    {
        private readonly DiscordClientService _clients;
        private readonly PamelloSpeakerService _speakers;
        private readonly PamelloPlayerRepository _players;

        public readonly SocketUser DiscordUser;
        public readonly PamelloCommandsModule Commands;

        public override int Id {
            get => Entity.Id;
        }
        public Guid Token {
            get => Entity.Token;
        }
        public DateTime JoinedAt {
            get => Entity.JoinedAt;
        }

        public int SongsPlayed {
            get => Entity.SongsPlayed;
            private set  {
                if (Entity.SongsPlayed == value) return;

                Entity.SongsPlayed = value;

                Save();

                //_events.UserEdited(this);
            }
        }

        public override string Name {
            get => DiscordUser.GlobalName;
            set => throw new Exception("Unable to change user name");
        }

        public bool IsAdministrator {
            get => Entity.IsAdministrator;
            set {
                if (Entity.IsAdministrator == value) return;

                Entity.IsAdministrator = value;
                Save();

                //_events.UserEdited(this);
            }
        }

        public IReadOnlyList<PamelloSong> AddedSongs {
            get => Entity.AddedSongs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }
        public IReadOnlyList<PamelloPlaylist> OwnedPlaylist {
            get => Entity.OwnedPlaylists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
        }
        public IReadOnlyList<PamelloSong> FavoriteSongs {
            get => Entity.FavoriteSongs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }
        public IReadOnlyList<PamelloPlaylist> FavoritePlaylists {
            get => Entity.FavoritePlaylists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
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

                //event
            }
        }
        public PamelloPlayer RequiredSelectedPlayer {
            get => EnsurePlayerExist();
        }

        public PamelloUser(IServiceProvider services,
            DatabaseUser databaseUser,
            SocketUser discordUser
        ) : base(databaseUser, services) {
            DiscordUser = discordUser;
            Commands = new PamelloCommandsModule(services, this);

            _clients = services.GetRequiredService<DiscordClientService>();
            _speakers = services.GetRequiredService<PamelloSpeakerService>();
            _players = services.GetRequiredService<PamelloPlayerRepository>();
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
                var playerId = Commands.PlayerCreate("Player").Result;
                player = _players.GetRequired(playerId);
            }

            SelectedPlayer = player;
            return player;
        }

        public void AddFavoriteSong(PamelloSong song) {
            if (Entity.FavoriteSongs.Any(databaseSong => databaseSong.Id == song.Id))
                throw new Exception("this song is already favorite");

            Entity.FavoriteSongs.Add(song.Entity);
            Save();
        }
        public void RemoveFavoriteSong(PamelloSong song) {
            if (!Entity.FavoriteSongs.Any(databaseSong => databaseSong.Id == song.Id))
                throw new Exception("this song is not favorite");

            Entity.FavoriteSongs.Remove(song.Entity);
            Save();
        }

        public void AddFavoritePlaylist(PamelloPlaylist playlist) {
            if (Entity.FavoritePlaylists.Any(databasePlaylist => databasePlaylist.Id == playlist.Id))
                throw new PamelloException("this playlist is already favorite");

            Entity.FavoritePlaylists.Add(playlist.Entity);
            Save();
        }
        public void RemoveFavoritePlaylist(PamelloPlaylist playlist) {
            if (!Entity.FavoritePlaylists.Any(databasePlaylist => databasePlaylist.Id == playlist.Id))
                throw new PamelloException("this playlist is not favorite");

            Entity.FavoritePlaylists.Remove(playlist.Entity);
            Save();
        }

        public PamelloPlaylist CreatePlaylist(string name) {
            return _playlists.Create(name, this);
        }

        public override object DTO => new { };
    }
}
