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
    private Task? _convertTask;

    protected override void InitAudioInternal(IServiceProvider services) {
        var dependencies = services.GetRequiredService<IDependenciesService>();
        var ffmpeg = dependencies.ResolveRequired("ffmpeg");

        Input.ProcessAudio = Process;

        _ffmpeg = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = ffmpeg.GetFile().FullName,
                Arguments = "-hide_banner -nostats -loglevel warning " +
                    "-f s16le -ac 2 -ar 48000 -re -i pipe:0 " +
                    "-acodec libmp3lame -b:a 320k -compression_level 0 -f mp3 pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _ffmpeg.ErrorDataReceived += (_, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data)) Framework.Logging.Output.Write($"ffmpeg: {e.Data}");
        };
        _ffmpeg.Start();
        _ffmpeg.BeginErrorReadLine();

        _convertTask = Task.Run(Writing);
    }


    private bool Process(byte[] audio, bool wait, CancellationToken token)
        => ProcessAsync(audio, wait, token).Result;
    private async Task<bool> ProcessAsync(byte[] audio, bool wait, CancellationToken token)
    {
        if (_ffmpeg?.HasExited ?? true) return false;

        try {
            await _ffmpeg.StandardInput.BaseStream.WriteAsync(audio, token);
            await _ffmpeg.StandardInput.BaseStream.FlushAsync(token);
        }
        catch (OperationCanceledException) {
            return false;
        }
        return true;
    }

    private async Task Writing() {
        var buffer = new byte[4096];

        while (_ffmpeg is { HasExited: false } ff) {
            int read;
            try {
                read = await ff.StandardOutput.BaseStream.ReadAsync(buffer);
            }
            catch (Exception ex) {
                Framework.Logging.Output.Write($"FFmpeg read error: {ex}");
                break;
            }
            if (read == 0) break;

            try {
                Output.Pass(buffer[..read], true, CancellationToken.None);
            }
            catch (Exception ex) {
                Framework.Logging.Output.Write($"FFmpeg output pass error: {ex}");
            }
        }

        Framework.Logging.Output.Write("FFmpeg reader stopped");
    }

    protected override void Dispose(bool isDisposing) {
        _ffmpeg?.Kill();
        _ffmpeg?.Dispose();
        _ffmpeg = null;
        
        base.Dispose(isDisposing);
    }
}