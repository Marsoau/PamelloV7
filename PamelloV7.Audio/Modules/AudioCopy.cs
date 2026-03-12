using PamelloV7.Audio.Points;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public class AudioCopy : IAudioModuleWithInput, IAudioModuleWithOutputs
{
    public List<IAudioPoint> Inputs { get; }
    public List<IAudioPoint> Outputs { get; }
    
    public IAudioPoint Input => Inputs.First();
    public int MinOutputs => 0;
    
    public AudioCopy() {
        Inputs = new List<IAudioPoint>(1);
        Outputs = new List<IAudioPoint>(10);
    }

    public void InitAudio(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        var result = Outputs.Aggregate(false, (isAny, output) => output.Pass(audio, wait, token) || isAny);
        return result;
    }
}
