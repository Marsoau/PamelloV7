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
    
    public abstract string GetSongArguments(string songKey);
    public abstract string GetPlaylistSongsArguments(string playlistKey);
    public abstract string GetPlaylistSongsKeysArguments(string playlistKey);

    public async Task RunYtDlp(string arguments, Func<StreamReader, Task>? outputHandler = null) {
        var ytDlp = _dependencies.ResolveRequired("yt-dlp");
        var ffmpeg = _dependencies.ResolveRequired("ffmpeg");
        var ffprobe = _dependencies.ResolveRequired("ffprobe");
    
        using var process = new Process();
        var startInfo = new ProcessStartInfo {
            FileName = ytDlp.GetFile().FullName,
            Arguments = arguments,
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

        if (outputHandler is not null) {
            await outputHandler(process.StandardOutput);
        }
    
        await process.WaitForExitAsync();
        var stderr = await stderrTask;
    
        if (process.ExitCode != 0) {
            throw new InvalidOperationException(
                $"yt-dlp exited with code {process.ExitCode}, stderr: {stderr}"
            );
        }
    }

    public async IAsyncEnumerable<string> GetYtDlpLines(string arguments) {
        StreamReader? ytReader = null;
        TaskCompletionSource tcs = new();
        
        var ytTask = RunYtDlp(arguments, reader => {
            ytReader = reader;
            tcs.SetResult();
            
            return Task.CompletedTask;
        });
        
        await tcs.Task;
        
        if (ytReader is null) throw new InvalidOperationException("Failed to start yt-dlp process");

        while (await ytReader.ReadLineAsync() is { } line) {
            yield return line;
        }
        
        await ytTask;
    }
    
    public async Task<YtDlpInfo> GetInfo(string key) {
        var stdout = "";
        
        await RunYtDlp(GetSongArguments(key), async reader => {
            stdout = await reader.ReadToEndAsync();
        });
        
        var info = JsonSerializer.Deserialize<YtDlpInfo>(stdout, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        });
    
        if (info is null) throw new InvalidOperationException($"Failed to deserialize yt-dlp output for key '{key}'.");
    
        return info;
    }

    public async IAsyncEnumerable<YtDlpInfo> GetPlaylistSongsInfos(string playlistKey) {
        await foreach (var line in GetYtDlpLines(GetPlaylistSongsArguments(playlistKey))) {
            var info = JsonSerializer.Deserialize<YtDlpInfo>(line);
            if (info is null) continue;
            
            yield return info;
        }
    }
    
    public async IAsyncEnumerable<string> GetPlaylistSongsKeysAsync(string playlistKey) {
        await foreach (var line in GetYtDlpLines(GetPlaylistSongsKeysArguments(playlistKey))) {
            yield return line;
        }
    }
}
