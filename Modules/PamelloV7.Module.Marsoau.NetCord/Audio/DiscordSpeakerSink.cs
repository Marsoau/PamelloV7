using PamelloV7.Framework.Audio.Modules.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Audio;

public partial class DiscordSpeakerSink : AudioModule, IAudioModuleWithInput
{
    public Stream? Stream { get; set; }

    protected override void InitAudioInternal(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        if (Stream is null) return false;
        
        Stream.Write(audio, 0, audio.Length);
        return true;
    }
}
