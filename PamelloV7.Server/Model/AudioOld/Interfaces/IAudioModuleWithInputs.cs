namespace PamelloV7.Server.Model.AudioOld.Interfaces;

public interface IAudioModuleWithInputs<TInput> : IAudioModule
    where TInput : IAudioPoint
{
    public int MinInputs { get; }
    public int MaxInputs { get; }
    
    public TInput CreateInput();
}