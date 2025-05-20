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
    
    private Process? _ffmpeg;
    
    public AudioPushPoint Input;
    public AudioPushPoint Output;
    
    public AudioPushPoint CreateInput()
    {
        Input = new AudioPushPoint();
        
        Input.Process = Process;
        
        return Input;
    }

    public AudioPushPoint CreateOutput()
    {
        Output = new AudioPushPoint();
        
        return Output;
    }
    public void InitModule()
    {
        _ffmpeg = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "ffmpeg",
                Arguments = "-f s16le -ac 2 -ar 48000 -re -i pipe:0 " +
                            "-acodec libmp3lame -b:a 128k -f mp3 pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _ffmpeg.Start();
        
        _ = Start();
    }

    private async Task<bool> Process(byte[] audio, bool wait)
    {
        if (_ffmpeg is null) return false;
        //Console.WriteLine($"ffmpeg got audio, is all 0: {audio.All(x => x == 0)}");
        
        await _ffmpeg.StandardInput.BaseStream.WriteAsync(audio);
        await _ffmpeg.StandardInput.BaseStream.FlushAsync();
        return true;
    }

    private async Task Start()
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
}