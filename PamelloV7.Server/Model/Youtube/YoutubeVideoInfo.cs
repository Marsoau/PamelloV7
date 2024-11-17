namespace PamelloV7.Server.Model.Youtube
{
    public class YoutubeVideoInfo
    {
        public string Name { get; set; }
        public string Channel { get; set; }
        public List<YoutubeEpisodeInfo> Episodes { get; set; }

        public override string ToString() {
            return $"{Channel}: {Name} ({Episodes.Count} episodes)";
        }
    }
}
