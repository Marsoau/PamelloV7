using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.SoundCloud.Platforms.Infos;

public class SoundCloudSongInfo : ISongInfo
{
    private YtDlpInfo _dlpInfo;
    
    public ISongPlatform Platform { get; }
    
    public string Key { get; }
    public string Name => _dlpInfo.Title;
    public string CoverUrl => _dlpInfo.Thumbnail ?? "";
    public List<IEpisodeInfo> Episodes { get; } = [];
    
    public SoundCloudSongInfo(ISongPlatform platform, YtDlpInfo dlpInfo) {
        Platform = platform;
        
        Key = Platform.ValueToKey(dlpInfo.WebpageUrl ?? "");
        
        _dlpInfo = dlpInfo;
    }
}
