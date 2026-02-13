using System.Diagnostics;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;
using PamelloV7.Server.Structures;

namespace PamelloV7.Audio.Modules;

public class AudioBuffer : IAudioModuleWithInput, IAudioModuleWithOutput
{
    public List<IAudioPoint> Inputs { get; }
    public List<IAudioPoint> Outputs { get; }
    public IAudioPoint Input => Inputs.First();
    public IAudioPoint Output => Outputs.First();
    
    private readonly RingBuffer<byte> _ring;
    
    public int Size => _ring.Buffer.Length;
    public int Head => _ring.Head;
    public int Tail => _ring.Tail;

    public AudioBuffer(int size = 1024) {
        Inputs = new List<IAudioPoint>(1);
        Outputs = new List<IAudioPoint>(1);
        
        _ring = new RingBuffer<byte>(size);
    }

    public void InitAudio() {
        Input.ProcessAudio = ProcessInput;
        Output.ProcessAudio = ProcessOutput;
    }

    private bool ProcessInput(byte[] audio, bool wait, CancellationToken token) {
        return _ring.WriteRange(audio, wait, token);
    }
    private bool ProcessOutput(byte[] buffer, bool wait, CancellationToken token) {
        return _ring.ReadRange(buffer, wait, token);
    }
}
