using PamelloV7.Audio.Points;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public class AudioSilence : IAudioModuleWithOutput
{
    public List<IAudioPoint> Outputs { get; }
    public IAudioPoint Output => Outputs.First();
    
    public AudioSilence() {
        Outputs = new List<IAudioPoint>(1);
    }

    public void InitAudio(IServiceProvider services) {
        Output.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        for (var i = 0; i < audio.Length; i++) audio[i] = 0;
        return true;
    }
}
