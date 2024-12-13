using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model
{
    public class PamelloEpisode : PamelloEntity<DatabaseEpisode>
    {
        public override int Id {
            get => Entity.Id;
        }
        public override string Name {
            get => Entity.Name;
            set {
                Entity.Name = value;
                Save();

                _events.Broadcast(new EpisodeNameUpdated() {
                    EpisodeId = Id,
                    Name = Name
                });
            }
        }
        public int Start {
            get => Entity.Start;
            set {
                if (Entity.Start == value) return;

                Entity.Start = value;
                Save();

                _events.Broadcast(new EpisodeStartUpdated() {
                    EpisodeId = Id,
                    Start = Start
                });
            }
        }
        public bool Skip {
            get => Entity.Skip;
            set {
                if (Entity.Skip == value) return;

                Entity.Skip = value;
                Save();

                _events.Broadcast(new EpisodeSkipUpdated() {
                    EpisodeId = Id,
                    Skip = Skip
                });
            }
        }

        public PamelloSong Song {
            get => _songs.GetRequired(Entity.Song.Id);
        }

        public PamelloEpisode(IServiceProvider services,
            DatabaseEpisode databaseEpisode
        ) : base(databaseEpisode, services) {

        }

        public override DiscordString ToDiscordString() {
            return DiscordString.Code(new AudioTime(Start).ToShortString()) + " - " + DiscordString.Url(Name, $"https://www.youtube.com/watch?v={Song.YoutubeId}&t={Start}") + " " + (Skip ? DiscordString.Italic("skip").ToString() : "");
        }

        public override string ToString() {
            return $"[{Id} ({(Skip ? "Skip" : "Play")})] {Name}";
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloEpisodeDTO() {
                Id = Id,
                Name = Name,
                Start = Start,
                Skip = Skip,
                SongId = Song.Id,
            };
        }
    }
}
