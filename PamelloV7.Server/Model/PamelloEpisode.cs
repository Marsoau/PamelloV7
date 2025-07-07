using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.DAL;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Server.Model
{
    public class PamelloEpisode : PamelloEntity<DatabaseEpisode>, IPamelloEpisode
    {
        private string _name;
        private AudioTime _start;
        private bool _autoSkip;
        private IPamelloSong _song;

        public override string Name {
            get => _name;
            set {
                _name = value;
                Save();

                _events.Broadcast(new EpisodeNameUpdated() {
                    EpisodeId = Id,
                    Name = Name
                });
            }
        }
        public AudioTime Start {
            get => _start;
            set {
                if (_start.TotalSeconds == value.TotalSeconds) return;

                _start = value;
                Save();

                _events.Broadcast(new EpisodeStartUpdated() {
                    EpisodeId = Id,
                    Start = Start.TotalSeconds
                });
            }
        }
        public bool AutoSkip {
            get => _autoSkip;
            set {
                if (_autoSkip == value) return;

                _autoSkip = value;
                Save();

                _events.Broadcast(new EpisodeSkipUpdated() {
                    EpisodeId = Id,
                    Skip = AutoSkip
                });
            }
        }

        public IPamelloSong Song {
            get => _song;
        }

        public PamelloEpisode(IServiceProvider services,
            DatabaseEpisode databaseEpisode
        ) : base(databaseEpisode, services) {
            _name = databaseEpisode.Name;
            _start = new AudioTime(databaseEpisode.Start);
            _autoSkip = databaseEpisode.Skip;
        }

        protected override void InitBase() {
            if (DatabaseEntity is null) return;

            _song = _songs.GetRequired(DatabaseEntity.Song.Id);
        }

        public override DiscordString ToDiscordString() {
            return (AutoSkip ? DiscordString.Code("skip").Italic() : DiscordString.Code(Start.ToShortString())) + " - " + DiscordString.Url(Name, $"https://www.youtube.com/watch?v={Song.YoutubeId}&t={Start}");
        }

        public override string ToString() {
            return $"[{Id} ({(AutoSkip ? "Skip" : "Play")})] {Name}";
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloEpisodeDTO() {
                Id = Id,
                Name = Name,
                Start = Start.TotalSeconds,
                Skip = AutoSkip,
                SongId = Song.Id,
            };
        }

        public override DatabaseEpisode GetDatabaseEntity(DatabaseContext? db = null) {
            db ??= GetDatabase();

            var dbEpisode = db.Episodes.Find(Id);
            if (dbEpisode is null) throw new PamelloException("Episode entity cant find itself in the database");

            dbEpisode.Name = Name;
            dbEpisode.Start = Start.TotalSeconds;
            dbEpisode.Skip = AutoSkip;

            return dbEpisode;
        }
    }
}
