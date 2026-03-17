using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public partial class AudioChoice : AudioModule, IAudioModuleWithOutput
{
    public override int MinInputs => 0;
    public override int MaxInputs => 10;

    protected override void InitAudioInternal(IServiceProvider services) {
        Output.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        foreach (var input in Inputs) {
            var result = input.Pass(audio, false, token);
            if (result) return true;
        }
        
        return false;
    }
}
