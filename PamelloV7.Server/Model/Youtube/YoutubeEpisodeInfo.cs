using PamelloV7.Framework.Audio;
using PamelloV7.Framework.Audio.Time;
using PamelloV7.Framework.AudioOld;

namespace PamelloV7.Server.Model.Youtube
{
    public class YoutubeEpisodeInfo
    {
        public string Name { get; set; }
        public int Start { get; set; }

        public override string ToString() {
            return $"[{new AudioTime(Start)}] {Name}";
        }
    }
}
