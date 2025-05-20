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
    public List<AudioPushPoint> Outputs;

    public AudioCopy() {
        Outputs = [];
    }
    
    public AudioPushPoint CreateInput() {
        Input = new AudioPushPoint();

        Input.Process = ProcessInput;

        return Input;
    }

    public AudioPushPoint CreateOutput() {
        var output = new AudioPushPoint();
        Console.WriteLine("create copy output");
        Outputs.Add(output);
        
        return output;
    }

    private async Task<bool> ProcessInput(byte[] audio, bool wait)
    {
        var currentOutputs = new List<AudioPushPoint>(Outputs);
        await Task.WhenAll(currentOutputs.Select(o => o.Push(audio, wait)));
        return true;
    }

    public void InitModule() {
    }
}