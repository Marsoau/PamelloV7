using Discord.WebSocket;
using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloUser : PamelloEntity<DatabaseUser>
    {
        public readonly SocketUser DiscordUser;

        public override int Id {
            get => _entity.Id;
        }
        public Guid Token {
            get => _entity.Token;
        }
        public DateTime JoinedAt {
            get => _entity.JoinedAt;
        }

        public int SongsPlayed {
            get => _entity.SongsPlayed;
            private set  {
                if (_entity.SongsPlayed == value) return;

                _entity.SongsPlayed = value;

                //_events.UserEdited(this);
            }
        }

        public override string Name {
            get => DiscordUser.GlobalName;
            set => throw new Exception("Unable to change user name");
        }

        public bool IsAdministrator {
            get => _entity.IsAdministrator;
            set {
                if (_entity.IsAdministrator == value) return;

                _entity.IsAdministrator = value;
                Save();

                //_events.UserEdited(this);
            }
        }

        public IReadOnlyList<PamelloSong> AddedSongs {
            get => _entity.AddedSongs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }
        public IReadOnlyList<PamelloPlaylist> OwnedPlaylist {
            get => _entity.OwnedPlaylists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
        }
        public IReadOnlyList<PamelloSong> FavoriteSongs {
            get => _entity.FavoriteSongs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }
        public IReadOnlyList<PamelloPlaylist> FavoritePlaylists {
            get => _entity.FavoritePlaylists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
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
