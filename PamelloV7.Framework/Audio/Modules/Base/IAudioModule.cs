using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModule : IDisposable
{
    public List<AudioPoint> Inputs { get; }
    
    public int MinInputs { get; }
    public int MaxInputs { get; }
    
    public List<AudioPoint> Outputs { get; }
    
    public int MinOutputs { get; }
    public int MaxOutputs { get; }

    public void InitAudio(IServiceProvider services);
    
    public AudioPoint AddInput();
    public AudioPoint AddOutput();

    public void RemoveInput();
    public void RemoveInput(AudioPoint point);
    public void RemoveInput(int index);
    
    public void RemoveOutput();
    public void RemoveOutput(AudioPoint point);
    public void RemoveOutput(int index);
}
