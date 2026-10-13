using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Audio.Modules;

public partial class AudioCopy : AudioModule, IAudioModuleWithInput
{
    public override int MinOutputs => 0;
    public override int MaxOutputs => 100;
    
    private readonly object _lock = new();

    protected override void InitAudioInternal(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    protected override void AddOutputInternal(AudioPoint p) { lock (_lock) base.AddOutputInternal(p); }
    protected override void RemoveOutputInternal(int i) { lock (_lock) base.RemoveOutputInternal(i); }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        AudioPoint[] outputs;
        
        lock (_lock) outputs = Outputs.ToArray();
        
        var any = false;
        foreach (var o in outputs) {
            try { any |= o.Pass(audio, wait, token); }
            catch (Exception x) { Framework.Logging.Output.Write($"Copy output failed: {x.Message}"); }
        }
        
        return any;
    }
}
