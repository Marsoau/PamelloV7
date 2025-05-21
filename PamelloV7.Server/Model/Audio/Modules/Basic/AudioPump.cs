using System.Runtime.CompilerServices;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioPump : IAudioModuleWithInputs<AudioPullPoint>, IAudioModuleWithOutputs<AudioPushPoint>
{
    public int MinInputs => 1;
    public int MaxInputs => 1;
    
    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioPullPoint Input;
    public AudioPushPoint Output;

    public Func<Task<bool>>? Condition;

    public int ChunkSize;

    public bool IsDisposed { get; private set; }

    public AudioPump() : this(2) {}
    public AudioPump(int chunkSize) {
        ChunkSize = chunkSize;
    }
    
    public AudioPullPoint CreateInput() {
        Input = new AudioPullPoint(this);
        
        return Input;
    }

    public AudioPushPoint CreateOutput() {
        Output = new AudioPushPoint(this);
        
        return Output;
    }
    
    public async Task Start() {
        var pair = new byte[ChunkSize];
        
        if (Condition is null) Condition = () => Task.FromResult(true);
        while (true) {
            if (!await Condition.Invoke())
            {
                Console.WriteLine("Condition was false, pump waits");
                await Task.Delay(1000);
                continue;
            }

            try
            {
                if (!await Input.Pull(pair, true))
                {
                    Console.WriteLine("PUMP Failed to puLL audio from input");
                    await Task.Delay(1000);
                }
                else if (!await Output.Push(pair, true))
                {
                    Console.WriteLine("PUMP Failed to puSH audio to output");
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Audio pump exception, some info:");
                Console.WriteLine($"Input: {Input}");
                Console.WriteLine($"Output: {Output}");
                Console.WriteLine($"Error info: {ex}");
                Console.WriteLine("Retrying in 3 seconds");
                await Task.Delay(3000);
            }
        }
    }

    public void InitModule() {
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        Input.Dispose();
        Output.Dispose();
    }
}