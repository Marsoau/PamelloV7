namespace PamelloV7.Server.Model.AudioOld.Interfaces;

public interface IAudioModuleWithModel : IAudioModule
{
    public AudioModel Model { get; }

    public void InitModel();
}