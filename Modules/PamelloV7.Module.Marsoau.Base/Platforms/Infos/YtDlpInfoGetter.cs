using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Dependencies.Service;

namespace PamelloV7.Module.Marsoau.Base.Platforms.Infos;

public abstract class YtDlpInfoGetter
{
    private readonly IServiceProvider _services;
    
    private readonly IDependenciesService _dependencies;
    
    public YtDlpInfoGetter(IServiceProvider services) {
        _services = services;
        
        _dependencies = services.GetRequiredService<IDependenciesService>();
    }
    
    public abstract string GetArguments(string key);

    public async Task<YtDlpInfo> GetInfo(string key) {
        var ytDlp = _dependencies.ResolveRequired("yt-dlp");
        var ffmpeg = _dependencies.ResolveRequired("ffmpeg");
        var ffprobe = _dependencies.ResolveRequired("ffprobe");
    
        using var process = new Process();
        var startInfo = new ProcessStartInfo {
            FileName = ytDlp.GetFile().FullName,
            Arguments = GetArguments(key),
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    
        var currentPath = startInfo.Environment["PATH"] ?? string.Empty;
        var sep = Path.PathSeparator;

        startInfo.Environment["PATH"] = $"{ffmpeg.GetDirectory().FullName}{sep}{ffprobe.GetDirectory().FullName}{sep}{currentPath}";
    
        process.StartInfo = startInfo;
    
        if (!process.Start()) {
            throw new InvalidOperationException("Failed to start yt-dlp process");
        }
    
        var stderrTask = process.StandardError.ReadToEndAsync();
        var stdout = await process.StandardOutput.ReadToEndAsync();
    
        await process.WaitForExitAsync();
        var stderr = await stderrTask;
    
        if (process.ExitCode != 0) {
            throw new InvalidOperationException(
                $"yt-dlp exited with code {process.ExitCode} for key '{key}', stderr: {stderr}"
            );
        }
    
        if (string.IsNullOrWhiteSpace(stdout)) {
            throw new InvalidOperationException(
                $"yt-dlp returned no output for key '{key}'. stderr: {stderr}"
            );
        }
    
        var info = JsonSerializer.Deserialize<YtDlpInfo>(stdout, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        });
    
        if (info is null) throw new InvalidOperationException($"Failed to deserialize yt-dlp output for key '{key}'.");
    
        return info;
    }
}
