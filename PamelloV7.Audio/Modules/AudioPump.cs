using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Audio.Modules;

public partial class AudioPump : AudioModule, IAudioModuleWithInput, IAudioModuleWithOutput
{
    public Func<bool> Condition { get; set; }
    
    private Task? _pumpTask;

    private readonly CancellationTokenSource _cts;

    private byte[] _buffer;

    public AudioPump(int bufferSize) {
        Condition = () => true;
        
        _cts = new CancellationTokenSource();
        
        _buffer = new byte[bufferSize];
    }

    public Task Start() {
        return _pumpTask = Task.Run(() => {
            while (!_cts.IsCancellationRequested) {
                try {
                    while (!Condition()) {
                        //StaticLogger.Log("PUMP CONDITION");
                        Task.Delay(1000).Wait();
                    }
                    
                    Pump();
                }
                catch (Exception x) {
                    Framework.Logging.Output.Write($"PUMP EXCEPTION: {x}");
                    Task.Delay(3000).Wait();
                    Framework.Logging.Output.Write("PUMP UNFROZEN");
                }
            }
        });
    }

    public void Pump() {
        while (!Input.Pass(_buffer, true, _cts.Token)) {
            Task.Delay(500).Wait();
            Framework.Logging.Output.Write("No input, waiting");
        }
        while (!Output.Pass(_buffer, true, _cts.Token)) {
            Task.Delay(500).Wait();
            Framework.Logging.Output.Write("No output, waiting");
        }
    }
}
