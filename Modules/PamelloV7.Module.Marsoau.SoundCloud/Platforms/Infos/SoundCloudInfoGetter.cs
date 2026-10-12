using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.Base.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.SoundCloud.Platforms.Infos;

public class SoundCloudInfoGetter : YtDlpInfoGetter
{
    private readonly ISongPlatform _platform;

    public SoundCloudInfoGetter(ISongPlatform platform, IServiceProvider services) : base(services) {
        _platform = platform;
    }
    
    public override string GetSongArguments(string songKey) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        SoundCloudSongPlatform.GetSoundCloudUrl(songKey)
    );

    public override string GetPlaylistSongsArguments(string playlistKey) => string.Join(' ',
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--dump-json",
        $@"--yes-playlist",
        $@"--ignore-errors",
        SoundCloudSongPlatform.GetSoundCloudPlaylistUrl(playlistKey)
    );

    public override string GetPlaylistSongsKeysArguments(string playlistKey) => string.Join(' ',
        $@"--quiet",
        $@"--no-warnings",
        $@"--skip-download",
        $@"--yes-playlist",
        $@"--ignore-errors",
        $@"--print ""%(webpage_url)s""",
        SoundCloudSongPlatform.GetSoundCloudPlaylistUrl(playlistKey)
    );

    public async Task<SoundCloudSongInfo> GetSongInfo(string key) {
        var info = await GetInfo(key);
        
        return new SoundCloudSongInfo(_platform, info);
    }

    public async IAsyncEnumerable<SoundCloudSongInfo> GetPlaylistSongsInfoAsync(string playlistKey) {
        await foreach (var info in GetPlaylistSongsInfos(playlistKey)) {
            yield return new SoundCloudSongInfo(_platform, info);
        }
    }
}
