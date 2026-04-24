using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

public class YoutubeInfoGetter : YtDlpInfoGetter
{
    private readonly ISongPlatform _platform;

    public YoutubeInfoGetter(ISongPlatform platform, IServiceProvider services) : base(services) {
        _platform = platform;
    }

    public override string GetArguments(string key) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        $@"https://www.youtube.com/watch?v={key}"
    );
    
    public async Task<YoutubeSongInfo> GetSongInfo(string key) {
        var info = await GetInfo(key);
        
        return new YoutubeSongInfo(_platform, info);
    }
}
