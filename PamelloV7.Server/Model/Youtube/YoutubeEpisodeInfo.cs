using PamelloV7.Core.Audio;

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
