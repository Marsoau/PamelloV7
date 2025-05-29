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
    
    private CancellationTokenSource _cts;
    
    public AudioFFmpeg(AudioModel parentModel)
    {
        ParentModel = parentModel;
        
        _cts = new CancellationTokenSource();
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
        
        _ = Writing();
    }

    private async Task<bool> Process(byte[] audio, bool wait)
    {
        if (_ffmpeg is null) return false;
        
        await _ffmpeg.StandardInput.BaseStream.WriteAsync(audio, _cts.Token);
        await _ffmpeg.StandardInput.BaseStream.FlushAsync(_cts.Token);
        return true;
    }

    private async Task Writing()
    {
        var buffer = new byte[4096];

        try {
            while (!(_ffmpeg?.HasExited ?? true)) {
                await _ffmpeg.StandardOutput.BaseStream.ReadAtLeastAsync(buffer, buffer.Length, true, _cts.Token);

                await Output.Push(buffer, true);
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Stream error: {ex}");
        }
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        _cts.Cancel();
        _ffmpeg?.Kill();
        _ffmpeg?.Dispose();
        Input.Dispose();
        Output.Dispose();
    }
}