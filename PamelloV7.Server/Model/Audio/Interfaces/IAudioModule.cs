namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioModule : IDisposable
{
    public AudioModel ParentModel { get; }
    
    public bool IsDisposed { get; }
    public void InitModule();
}