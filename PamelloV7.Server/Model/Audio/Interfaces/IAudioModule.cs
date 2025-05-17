namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioModule
{
    public int MaxInputs { get; }
    public int MaxOutputs { get; }
    
    public List<IAudioPoint> GetInputs();
    public List<IAudioPoint> GetOutputs();
}