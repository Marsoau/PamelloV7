using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public partial class AudioCopy : AudioModule, IAudioModuleWithInput
{
    public override int MinOutputs => 0;
    public override int MaxOutputs => 10;

    protected override void InitAudioInternal(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        var result = Outputs.Aggregate(false, (isAny, output) => output.Pass(audio, wait, token) || isAny);
        return result;
    }
}
