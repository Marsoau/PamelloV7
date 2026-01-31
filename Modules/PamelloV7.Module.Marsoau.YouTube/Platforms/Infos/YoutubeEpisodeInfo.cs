using PamelloV7.Core.Audio;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos
{
    public class YoutubeEpisodeInfo : IEpisodeInfo
    {
        public ISongInfo SongInfo { get; }
        
        public string Name { get; set; }
        public int Start { get; set; }
        
        public YoutubeEpisodeInfo(ISongInfo songInfo) {
            SongInfo = songInfo;
        }

        public override string ToString() {
            return $"[{new AudioTime(Start)}] {Name}";
        }
    }
}
