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

namespace PamelloV7.Module.Marsoau.YouTube.Downloaders;

[SongDownloader("youtube")]
public class YoutubeSongDownloader : YtDlpDownloader
{
    public YoutubeSongDownloader(IServiceProvider services, SongSource source) : base(services, source) { }

    public override string GetArguments(FileInfo file) => string.Join(' ',
        //$@"--plugin-dirs ""/home/marsoau/.config/yt-dlp/plugins""",
        $@"--extractor-args ""youtube:player_client=android""",
        $@"--quiet",
        //$@"--verbose",
        $@"--newline",
        $@"--progress",
        $@"--no-wait-for-video",
        $@"--no-keep-video",
        $@"--no-audio-multistreams",
        $@"--extract-audio",
        $@"--output ""{file.FullName}""",
        $@"--audio-format opus",
        $@"--progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s""",
        $@"https://www.youtube.com/watch?v={Source.PK.Key}"
    );
}
