using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Module.Marsoau.Base.Platforms.Downloaders;
using PamelloV7.Module.Marsoau.SoundCloud.Platforms;

namespace PamelloV7.Module.Marsoau.SoundCloud.Downloaders;

[SongDownloader("soundcloud")]
public class SoundCloudSongDownloader : YtDlpDownloader
{
    public SoundCloudSongDownloader(IServiceProvider services, SongSource source) : base(services, source) { }

    public override string GetArguments(FileInfo file) => string.Join(' ',
        $@"--quiet",
        $@"--newline",
        $@"--progress",
        $@"--no-playlist",
        $@"--no-wait-for-video",
        $@"--no-keep-video",
        $@"--no-audio-multistreams",
        $@"--extract-audio",
        $@"--output ""{file.FullName}""",
        $@"--audio-format opus",
        $@"--progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s""",
        SoundCloudSongPlatform.GetSoundCloudUrl(Source.PK.Key)
    );
}
