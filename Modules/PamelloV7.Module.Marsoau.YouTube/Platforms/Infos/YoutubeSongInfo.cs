using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

public class YoutubeSongInfo : ISongInfo
{
    private YtDlpInfo _dlpInfo;
    
    public ISongPlatform Platform { get; }
    
    public string Key => _dlpInfo.Id;
    public string Name => _dlpInfo.Title;
    public string CoverUrl => _dlpInfo.Thumbnail ?? "";
    public List<IEpisodeInfo> Episodes { get; }

    public YoutubeSongInfo(ISongPlatform platform, YtDlpInfo dlpInfo) {
        Platform = platform;
        
        _dlpInfo = dlpInfo;

        Episodes = (_dlpInfo.Chapters ?? []).Select(chapter => new YoutubeEpisodeInfo(this) {
            Name = chapter.Title,
            Start = (int)chapter.StartTime
        }).OfType<IEpisodeInfo>().ToList();
    }
}
