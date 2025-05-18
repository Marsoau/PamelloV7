namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioModuleWithOutputs<TOutput> : IAudioModule
    where TOutput : IAudioPoint
{
    public int MinOutputs { get; }
    public int MaxOutputs { get; }
    
    public TOutput CreateOutput();
}