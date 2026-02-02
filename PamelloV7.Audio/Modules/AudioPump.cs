using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Audio.Modules;

public class AudioPump : IAudioModuleWithInput, IAudioModuleWithOutput
{
    public List<IAudioPoint> Inputs { get; }
    public List<IAudioPoint> Outputs { get; }
    
    public IAudioPoint Input => Inputs.First();
    public IAudioPoint Output => Outputs.First();
    
    private Task? _pumpTask;

    private readonly CancellationTokenSource _cts;

    private byte[] _buffer;

    public AudioPump(int bufferSize) {
        Inputs = new List<IAudioPoint>(1);
        Outputs = new List<IAudioPoint>(1);
        
        _cts = new CancellationTokenSource();
        
        _buffer = new byte[bufferSize];
    }

    public Task Start() {
        return _pumpTask = Task.Run(() => {
            while (!_cts.IsCancellationRequested) Pump();
        });
    }

    public void Pump() {
        while (!Input.Pass(_buffer, true, _cts.Token)) {
            Console.WriteLine("No input, waiting");
            Task.Delay(500).Wait();
        }
        while (!Output.Pass(_buffer, true, _cts.Token)) {
            Console.WriteLine("No output, waiting");
            Task.Delay(500).Wait();
        }
    }
}
