using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Module.Marsoau.Base.Platforms.Downloaders;
using PamelloV7.Module.Marsoau.YouTube.Platforms;

namespace PamelloV7.Module.Marsoau.YouTube.Downloaders;

[SongDownloader("youtube")]
public class YoutubeSongDownloader : YtDlpDownloader
{
    public YoutubeSongDownloader(IServiceProvider services, SongSource source) : base(services, source) { }

    public override string GetArguments(FileInfo file) => string.Join(' ',
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        $@"--newline",
        $@"--progress",
        $@"--no-wait-for-video",
        $@"--no-keep-video",
        $@"--no-audio-multistreams",
        $@"--extract-audio",
        $@"--output ""{file.FullName}""",
        $@"--audio-format opus",
        $@"--progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s""",
        YoutubeSongPlatform.GetYoutubeUrl(Source.PK.Key)
    );
}
