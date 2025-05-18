using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Inputs;

public class AudioSilence : IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioPullPoint Output;

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint();
        
        Output.OnRequest += Request;
        
        return Output;
    }

    private Task Request(byte[] buffer) {
        buffer.AsSpan().Clear();
        return Task.CompletedTask;
    }

    public void InitModule() {
    }
}