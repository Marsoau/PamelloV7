using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;

namespace PamelloV7.Server.Model.AudioOld.Modules.Basic;

public class AudioChoise : IAudioModuleWithInputs<AudioPullPoint>, IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinInputs => 0;
    public int MaxInputs => 100;

    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioModel ParentModel { get; }
    
    public List<AudioPullPoint> Inputs;
    public AudioPullPoint Output;

    public bool IsDisposed { get; private set; }

    public AudioChoise(AudioModel parentModel)
    {
        ParentModel = parentModel;

        Inputs = [];
    }
    
    public AudioPullPoint CreateInput() {
        var input = new AudioPullPoint(this);
        
        Inputs.Add(input);
        
        return input;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint(this);
        
        Output.OnRequest += Request;
        
        return Output;
    }

    public void InitModule() {
    }

    private async Task<bool> Request(byte[] buffer, bool wait, CancellationToken token) {
        for (var i = 0; i < Inputs.Count; i++)
        {
            if (await Inputs[i].Pull(buffer, wait && Inputs.Count - 1 == i, token))
            {
                return true;
            }
        }

        return false;
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        Output.Dispose();
        Inputs.ForEach(i => i.Dispose());
    }
}