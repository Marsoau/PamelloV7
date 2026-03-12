using PamelloV7.Audio.Points;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public class AudioChoice : IAudioModuleWithInputs, IAudioModuleWithOutput
{
    public int MinInputs => 0;
    public List<IAudioPoint> Inputs { get; }
    public List<IAudioPoint> Outputs { get; }
    
    public IAudioPoint Output => Outputs.First();

    public AudioChoice() {
        Inputs = new List<IAudioPoint>(10);
        Outputs = new List<IAudioPoint>(1);
    }

    public void InitAudio(IServiceProvider services) {
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
