using AngleSharp.Io;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioChoise : IAudioModuleWithInputs<AudioPullPoint>, IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinInputs { get; }
    public int MaxInputs { get; }
    
    public int MinOutputs { get; }
    public int MaxOutputs { get; }
    
    public AudioModel ParentModel { get; }
    
    public List<AudioPullPoint> Inputs;
    public AudioPullPoint Output;

    public bool IsDisposed { get; private set; }

    public AudioChoise(AudioModel parentModel)
    {
        ParentModel = parentModel;
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

    private async Task<bool> Request(byte[] buffer, bool wait) {
        for (var i = 0; i < Inputs.Count; i++)
        {
            if (await Inputs[i].Pull(buffer, wait && Inputs.Count - 1 == i))
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