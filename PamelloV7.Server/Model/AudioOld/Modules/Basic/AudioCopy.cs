using System.Collections.Concurrent;
using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;

namespace PamelloV7.Server.Model.AudioOld.Modules.Basic;

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

    public bool DeleteOnFail;

    public AudioCopy(AudioModel parentModel, bool deleteOnFail) {
        ParentModel = parentModel;
        DeleteOnFail = deleteOnFail;
        
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

    private async Task<bool> ProcessInput(byte[] audio, bool wait, CancellationToken token)
    {
        var anySuccess = false;
        foreach (var output in Outputs.Values)
        {
           if (await PushToPoint(output, audio, wait, token) && !anySuccess) anySuccess = true;
        }
        
        return anySuccess;
        // old // await Task.WhenAll(Outputs.Select(kvp => PushToPoint(kvp.Value, audio, wait)));
    }

    private async Task<bool> PushToPoint(AudioPushPoint point, byte[] audio, bool wait, CancellationToken token)
    {
        if (await point.Push(audio, wait, token)) return true;
        if (!DeleteOnFail) return false;
        
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