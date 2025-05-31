using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Pipes;

public class AudioFFmpeg : IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithOutputs<AudioPushPoint>
{

    public int MinInputs => 1;
    public int MaxInputs => 1;
    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioModel ParentModel { get; }
    
    public AudioPushPoint Input;
    public AudioPushPoint Output;
    
    private Process? _ffmpeg;
    
    private CancellationTokenSource _writingCTS;
    private Task _writingTask;
    
    public AudioFFmpeg(AudioModel parentModel)
    {
        ParentModel = parentModel;
        
        _writingCTS = new CancellationTokenSource();
    }
    
    public AudioPushPoint CreateInput()
    {
        Input = new AudioPushPoint(this);
        
        Input.Process = Process;
        
        return Input;
    }

    public AudioPushPoint CreateOutput()
    {
        Output = new AudioPushPoint(this);
        
        return Output;
    }

    public bool IsDisposed { get; private set; }

    public void InitModule()
    {
        _ffmpeg = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "ffmpeg",
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
        
        _writingTask = Writing();
    }

    private async Task<bool> Process(byte[] audio, bool wait, CancellationToken token)
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
                await _ffmpeg.StandardOutput.BaseStream.ReadAtLeastAsync(buffer, buffer.Length, true,
                    _writingCTS.Token);

                await Output.Push(buffer, true, _writingCTS.Token);
            }
        }
        catch (OperationCanceledException) {
            Console.WriteLine("FFMpeg writing task was canceled");
            return;
        }
        catch (Exception ex) {
            Console.WriteLine($"Stream error: {ex}");
        }
    }

    public void Dispose()
    {
        if (IsDisposed) return;
        else IsDisposed = true;
        
        _writingCTS.Cancel();
        
        _ffmpeg?.Kill();
        _ffmpeg?.Dispose();
        Input.Dispose();
        Output.Dispose();
        
        _writingTask.Wait();
    }
}