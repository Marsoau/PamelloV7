using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;

namespace PamelloV7.Server.Model.AudioOld.Modules.Inputs;

public class AudioSilence : IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioModel ParentModel { get; }
    
    public AudioPullPoint Output;

    public AudioSilence(AudioModel parentModel)
    {
        ParentModel = parentModel;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint(this);
        
        Output.OnRequest += Request;
        
        return Output;
    }

    private async Task<bool> Request(byte[] buffer, bool wait, CancellationToken token) {
        buffer.AsSpan().Clear();
        return true;
    }

    public bool IsDisposed { get; private set; }

    public void InitModule() {
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        Output.Dispose();
    }
}