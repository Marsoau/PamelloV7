using System.Diagnostics;
using System.Text;
using System.Text.Json;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Module.Marsoau.YouTube.Downloaders;

[SongDownloader("youtube")]
public class YoutubeSongDownloader : SongDownloader
{
    public YoutubeSongDownloader(IServiceProvider services, SongSource source) : base(services, source) { }
    
    protected override async Task<EDownloadResult> InternalDownloadAsync(FileInfo file) {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo() {
            FileName = "yt-dlp",
            Arguments = string.Join(' ',
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
            ),
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            EnvironmentVariables = { ["PYTHONPATH"] = "/home/marsoau/.config/yt-dlp/plugins" }
        };

        if (!process.Start()) {
            return EDownloadResult.CantStart;
        }

        var reader = process.StandardOutput;

        string? line;
        while ((line = await reader.ReadLineAsync()) is not null) 
        {
            ProcessProgressLine(line);
        }
        
        await process.WaitForExitAsync();
        
        var finalResult = process.ExitCode == 0 ? EDownloadResult.Success : EDownloadResult.UnknownError;
        
        return finalResult;
    }

    private void ProcessProgressLine(string line) {
        var parts = line.Split('/');
        if (parts.Length < 2) return;

        if (!long.TryParse(parts[0], out var down) || !long.TryParse(parts[1], out var total)) return;
        if (total > 0) Progress = (double)down / total;
    }
}
