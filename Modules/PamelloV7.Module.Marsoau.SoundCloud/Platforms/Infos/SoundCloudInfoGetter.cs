using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.SoundCloud.Platforms.Infos;

public class SoundCloudInfoGetter : YtDlpInfoGetter
{
    private readonly ISongPlatform _platform;

    public SoundCloudInfoGetter(ISongPlatform platform, IServiceProvider services) : base(services) {
        _platform = platform;
    }
    
    public override string GetArguments(string key) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        SoundCloudSongPlatform.GetSoundCloudUrl(key)
    );

    public async Task<SoundCloudSongInfo> GetSongInfo(string key) {
        var info = await GetInfo(key);
        
        return new SoundCloudSongInfo(_platform, info);
    }
}
