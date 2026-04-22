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
    
    public Action OnNoInput { get; set; } = () => { };
    public Action OnNoOutput { get; set; } = () => { };
    
    public bool WaitOnInput { get; set; } = true;
    public bool WaitOnOutput { get; set; } = true;

    public AudioPump(int bufferSize) {
        Condition = () => true;
        
        _cts = new CancellationTokenSource();
        
        _buffer = new byte[bufferSize];
    }

    public Task Start() {
        return _pumpTask = Task.Run(() => {
            while (!_cts.IsCancellationRequested) {
                try {
                    while (!Condition() && !_cts.IsCancellationRequested) {
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

    public async Task Stop() {
        if (_pumpTask is null) return;
        
        await _cts.CancelAsync();

        await Task.WhenAny(Task.Delay(10000), _pumpTask);
    }

    public void Pump() {
        while (!_cts.Token.IsCancellationRequested && !Input.Pass(_buffer, WaitOnInput, _cts.Token)) {
            OnNoInput();
            Task.Delay(500).Wait();
            //Framework.Logging.Output.Write("No input, waiting");
        }
        while (!_cts.Token.IsCancellationRequested && !Output.Pass(_buffer, WaitOnOutput, _cts.Token)) {
            OnNoOutput();
            Task.Delay(500).Wait();
            //Framework.Logging.Output.Write("No output, waiting");
        }
    }

    protected override void Dispose(bool isDisposing) {
        Stop().Wait();
        
        base.Dispose(isDisposing);
    }
}
