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
    
    public AudioModel ParentModel { get; }
    
    public AudioPullPoint Input;
    public AudioPushPoint Output;

    public Func<Task<bool>>? Condition;

    private CancellationTokenSource _cts;
    private Task _pumpTask;

    public int ChunkSize;

    public bool IsDisposed { get; private set; }

    public AudioPump(AudioModel parentModel) : this(parentModel, 2) {}
    public AudioPump(AudioModel parentModel, int chunkSize) {
        ParentModel = parentModel;
        
        ChunkSize = chunkSize;
        _cts = new CancellationTokenSource();
    }
    
    public AudioPullPoint CreateInput() {
        Input = new AudioPullPoint(this);
        
        return Input;
    }

    public AudioPushPoint CreateOutput() {
        Output = new AudioPushPoint(this);
        
        return Output;
    }

    public void Start()
    {
        _pumpTask = StartAsync();
    }
    private async Task StartAsync() {
        var pair = new byte[ChunkSize];
        
        if (Condition is null) Condition = () => Task.FromResult(true);
        
        while (!_cts.IsCancellationRequested) {
            if (!await Condition.Invoke())
            {
                Console.WriteLine("Condition was false, pump waits");
                await Task.Delay(1000, _cts.Token);
                continue;
            }

            try
            {
                while (!await Input.Pull(pair, true))
                {
                    Console.WriteLine($"PUMP Failed to puLL audio from input {GetHashCode()}");
                    await Task.Delay(1000, _cts.Token);
                }
                while (!await Output.Push(pair, true))
                {
                    Console.WriteLine($"PUMP Failed to puSH audio to output {GetHashCode()}");
                    await Task.Delay(1000, _cts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Audio pump exception, some info:");
                Console.WriteLine($"Input: {Input}");
                Console.WriteLine($"Output: {Output}");
                Console.WriteLine($"Error info: {ex}");
                Console.WriteLine("Retrying in 3 seconds");
                await Task.Delay(3000, _cts.Token);
            }
        }
    }

    public void InitModule() {
    }

    public void Dispose()
    {
        Console.WriteLine("DISPOSE CALLED");
        IsDisposed = true;
        
        _cts.Cancel();
        _pumpTask.Wait();
        
        Input.Dispose();
        Output.Dispose();
    }
}