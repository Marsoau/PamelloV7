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
        
        Outputs.Add(output);
        
        return output;
    }

    private Task ProcessInput(byte[] audio) {
        return Task.WhenAll(Outputs.Select(o => o.Push(audio)));
    }

    public void InitModule() {
    }
}