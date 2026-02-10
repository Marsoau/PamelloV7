using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Module.Marsoau.Discord.Audio;

public class SpeakerAudioOutput : IAudioModuleWithInput
{
    public List<IAudioPoint> Inputs { get; }
    public IAudioPoint Input => Inputs.First();
    
    public Stream? Stream { get; set; }
    
    public SpeakerAudioOutput() {
        Inputs = new List<IAudioPoint>(1);
    }

    public void InitAudio() {
        Input.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        if (Stream is null) return false;
        
        Stream.Write(audio, 0, audio.Length);
        return true;
    }
}
