using PamelloV7.Core.Audio;
using PamelloV7.Framework.Audio;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos
{
    public class YoutubeEpisodeInfo : IEpisodeInfo
    {
        public ISongInfo SongInfo { get; }
        
        public required string Name { get; set; }
        public required int Start { get; set; }
        
        public YoutubeEpisodeInfo(ISongInfo songInfo) {
            SongInfo = songInfo;
        }

        public override string ToString() {
            return $"[{new AudioTime(Start)}] {Name}";
        }
    }
}
