using Discord.WebSocket;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Audio;

namespace PamelloV7.Server.Model
{
    public class PamelloUser : PamelloEntity<DatabaseUser>
    {
        public readonly SocketUser DiscordUser;

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

        private PamelloPlayer? _selectedPlayer;
        public PamelloPlayer? SelectedPlayer {
            get => _selectedPlayer;
            set {
                if (_selectedPlayer == value) return;

                _selectedPlayer = value;

                //event
            }
        }

        public PamelloUser(IServiceProvider services,
            DatabaseUser databaseUser,
            SocketUser discordUser
        ) : base(databaseUser, services) {
            DiscordUser = discordUser;
        }

        public override object GetDTO() {
            return new {

            };
        }
    }
}
