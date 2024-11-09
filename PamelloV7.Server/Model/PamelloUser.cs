using Discord.WebSocket;
using PamelloV7.DAL.Entity;

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
            private set => Entity.SongsPlayed = value;
        }

        public override string Name {
            get => DiscordUser.GlobalName;
            set => throw new Exception("Unable to change username");
        }

        public bool IsAdministrator {
            get => Entity.IsAdministrator;
            set {
                if (Entity.IsAdministrator == value) return;

                Entity.IsAdministrator = value;
                Save();
            }
        }

        public PamelloUser(IServiceProvider services,
            DatabaseUser databaseEntity,
            SocketUser discordUser
        ) : base(databaseEntity, services) {
            DiscordUser = discordUser;
        }

        public override object GetDTO() {
            return new {

            };
        }
    }
}
