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
    
    public AudioFFmpeg(AudioModel parentModel)
    {
        ParentModel = parentModel;
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
                            //"-acodec libmp3lame -b:a 128k -f mp3 pipe:1",
                            "-acodec libmp3lame -b:a 320k -q:a 0 -compression_level 0 -f mp3 pipe:1",
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
        
        await _ffmpeg.StandardInput.BaseStream.WriteAsync(audio);
        await _ffmpeg.StandardInput.BaseStream.FlushAsync();
        return true;
    }

    private async Task Writing()
    {
        var buffer = new byte[4096];
        var stream = _ffmpeg!.StandardOutput.BaseStream;

        try {
            while (true) {
                var read = await stream.ReadAsync(buffer);
                if (read == 0) break;

                var data = buffer[..read];

                await Output.Push(data, true);
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Stream error: {ex}");
        }
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        _ffmpeg?.Dispose();
        Input.Dispose();
        Output.Dispose();
    }
}