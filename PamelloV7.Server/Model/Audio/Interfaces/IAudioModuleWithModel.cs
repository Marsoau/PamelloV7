namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioModuleWithModel : IAudioModule
{
    public AudioModel Model { get; }

    public void InitModel();
}