using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

public class YoutubeInfoGetter : YtDlpInfoGetter
{
    private readonly ISongPlatform _platform;

    public YoutubeInfoGetter(ISongPlatform platform, IServiceProvider services) : base(services) {
        _platform = platform;
    }

    public override string GetSongArguments(string songKey) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        YoutubeSongPlatform.GetYoutubeSongUrl(songKey)
    );
    
    public override string GetPlaylistSongsArguments(string playlistKey) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        $@"--yes-playlist",
        $@"--ignore-errors",
        YoutubeSongPlatform.GetYoutubePlaylistUrl(playlistKey)
    );

    public override string GetPlaylistSongsKeysArguments(string playlistKey) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--flat-playlist",
        $@"--print ""%(id)s""",
        YoutubeSongPlatform.GetYoutubePlaylistUrl(playlistKey)
    );

    public async Task<YoutubeSongInfo> GetSongInfoAsync(string key) {
        var info = await GetInfo(key);
        
        return new YoutubeSongInfo(_platform, info);
    }

    public async IAsyncEnumerable<YoutubeSongInfo> GetPlaylistSongsInfoAsync(string playlistKey) {
        await foreach (var info in GetPlaylistSongsInfos(playlistKey)) {
            yield return new YoutubeSongInfo(_platform, info);
        }
    }
}
