using PamelloV7.Core.Platforms;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

public class YoutubeVideoInfo : ISongInfo
{
    public ISongPlatform Platform { get; }
    
    public string Key { get; set; }
    public string Name => Title;
    public string CoverUrl { get; set; }
    
    public string Title { get; set; }
    public string Channel { get; set; }

    public List<IEpisodeInfo> Episodes { get; set; }

    public YoutubeVideoInfo(ISongPlatform platform, string key) {
        Platform = platform;
        Key = key;
    }

    public override string ToString() {
        return $"[{Key}] {Channel}: {Name} ({Episodes.Count} episodes)\n| {string.Join("\n| ", Episodes)}";
    }
}
