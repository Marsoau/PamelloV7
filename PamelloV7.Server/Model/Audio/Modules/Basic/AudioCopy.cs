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

    public bool IsDisposed { get; private set; }

    public AudioCopy() {
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
        await Task.WhenAll(Outputs.Select(kvp => PushToPoint(kvp.Value, audio, wait)));
        return true;
    }

    private async Task PushToPoint(AudioPushPoint point, byte[] audio, bool wait)
    {
        if (await point.Push(audio, wait)) return;
        
        Outputs.TryRemove(point.Id, out _);
        point.Dispose();
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