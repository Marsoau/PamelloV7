using System.Collections.Concurrent;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioCopy : IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithOutputs<AudioPushPoint>
{
    public int MinInputs => 1;
    public int MaxInputs => 1;
    
    public int MinOutputs => 0;
    public int MaxOutputs => 100;

    public AudioPushPoint Input;
    public ConcurrentDictionary<int, AudioPushPoint> Outputs;

    public AudioModel ParentModel { get; }
    public bool IsDisposed { get; private set; }

    public AudioCopy(AudioModel parentModel) {
        ParentModel = parentModel;
        
        Outputs = [];
    }
    
    public AudioPushPoint CreateInput() {
        Input = new AudioPushPoint(this);

        Input.Process = ProcessInput;

        return Input;
    }

    public AudioPushPoint CreateOutput() {
        var output = new AudioPushPoint(this);
        Console.WriteLine("create copy output");
        var atempts = 3;
        while (atempts > 0 && !Outputs.TryAdd(output.Id, output)) atempts--;
        
        return output;
    }

    private async Task<bool> ProcessInput(byte[] audio, bool wait)
    {
        if (Outputs.Count == 0) return false;

        var tasks = Outputs.Select(kvp => PushToPoint(kvp.Value, audio, wait));
        await Task.WhenAll(tasks);
        var result = tasks.Any(task => task.Result);
        // Console.WriteLine($"Result of copy {GetHashCode()}: {result}");
        return result;
    }

    private async Task<bool> PushToPoint(AudioPushPoint point, byte[] audio, bool wait)
    {
        if (await point.Push(audio, wait)) return true;
        
        Outputs.TryRemove(point.Id, out _);
        point.Dispose();

        return false;
    }

    public void InitModule() {
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        Input.Dispose();
        foreach (var output in Outputs.Values) output.Dispose();
    }
}