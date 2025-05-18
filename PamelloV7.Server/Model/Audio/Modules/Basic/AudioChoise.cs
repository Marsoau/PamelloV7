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
    
    public List<AudioPullPoint> Inputs;
    public AudioPullPoint Output;
    
    public AudioPullPoint CreateInput() {
        var input = new AudioPullPoint();
        
        Inputs.Add(input);
        
        return input;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint();
        
        Output.OnRequest += Request;
        
        return Output;
    }
    
    public void InitModule() {
    }

    private Task Request(byte[] arg) {
        throw new NotImplementedException();
    }
}