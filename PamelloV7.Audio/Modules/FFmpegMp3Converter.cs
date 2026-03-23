using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Audio.Modules;

public partial class FFmpegMp3Converter : AudioModule, IAudioModuleWithInput, IAudioModuleWithOutput
{
    private Process? _ffmpeg;
    private Task _convertTask;

    protected override void InitAudioInternal(IServiceProvider services) {
        var dependencies = services.GetRequiredService<IDependenciesService>();
        var ffmpeg = dependencies.ResolveRequired("ffmpeg");
        
        Input.ProcessAudio = Process;
        
        _ffmpeg = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = ffmpeg.GetFile().FullName,
                Arguments = "-f s16le -ac 2 -ar 48000 -re -i pipe:0 " +
                            "-acodec libmp3lame -b:a 320k -compression_level 0 -f mp3 pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _ffmpeg.Start();
        
        _convertTask = Task.Run(Writing);
    }

    private bool Process(byte[] audio, bool wait, CancellationToken token)
        => ProcessAsync(audio, wait, token).Result;
    private async Task<bool> ProcessAsync(byte[] audio, bool wait, CancellationToken token)
    {
        if (_ffmpeg is null) return false;

        try {
            await _ffmpeg.StandardInput.BaseStream.WriteAsync(audio, token);
            await _ffmpeg.StandardInput.BaseStream.FlushAsync(token);
        }
        catch (OperationCanceledException) {
            return false;
        }
        return true;
    }

    private async Task Writing()
    {
        var buffer = new byte[4096];

        try {
            while (!(_ffmpeg?.HasExited ?? true)) {
                await _ffmpeg.StandardOutput.BaseStream.ReadAtLeastAsync(buffer, buffer.Length);

                Output.Pass(buffer, true, CancellationToken.None);
            }
        }
        catch (OperationCanceledException) {
            StaticLogger.Log("FFMpeg writing task was canceled");
            return;
        }
        catch (Exception ex) {
            StaticLogger.Log($"Stream error: {ex}");
        }
    }
}