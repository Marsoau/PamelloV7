using System.Diagnostics;
using System.Text;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Module.Marsoau.Base.Platforms.Downloaders;

public abstract class YtDlpDownloader : SongDownloader
{
    public YtDlpDownloader(IServiceProvider services, SongSource source) : base(services, source) { }

    public abstract string GetArguments(FileInfo file);
    
    protected override async Task<EDownloadResult> InternalDownloadAsync(FileInfo file) {
        var ytDlp = _dependencies.ResolveRequired("yt-dlp");
        var ffmpeg = _dependencies.ResolveRequired("ffmpeg");
        var ffprobe = _dependencies.ResolveRequired("ffprobe");
        
        using var process = new Process();
        var startInfo = new ProcessStartInfo {
            FileName = ytDlp.GetFile().FullName,
            Arguments = GetArguments(file),
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            EnvironmentVariables = {
                ["PYTHONPATH"] = "/home/marsoau/.config/yt-dlp/plugins",
            }
        };
        
        var currentPath = startInfo.Environment["PATH"] ?? string.Empty;
        var sep = Path.PathSeparator;
    
        startInfo.Environment["PATH"] = $"{ffmpeg.GetDirectory().FullName}{sep}{ffprobe.GetDirectory().FullName}{sep}{currentPath}";
        
        process.StartInfo = startInfo;

        if (!process.Start()) {
            return EDownloadResult.CantStart;
        }

        var stderrTask = process.StandardError.ReadToEndAsync();
        
        var reader = process.StandardOutput;

        string? line;
        while ((line = await reader.ReadLineAsync()) is not null) 
        {
            ProcessProgressLine(line);
        }
        
        await process.WaitForExitAsync();
        
        var stderr = await stderrTask;
        
        if (process.ExitCode != 0) {
            throw new PamelloException($"yt-dlp exited with code {process.ExitCode}: {stderr.Trim()}");
        }
        
        return EDownloadResult.Success;
    }

    private void ProcessProgressLine(string line) {
        var parts = line.Split('/');
        if (parts.Length < 2) return;

        if (!long.TryParse(parts[0], out var down) || !long.TryParse(parts[1], out var total)) return;
        if (total > 0) Progress = (double)down / total;
    }
}
