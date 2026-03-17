using System.Diagnostics;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Server.Structures;

namespace PamelloV7.Audio.Modules;

public partial class AudioBuffer : AudioModule, IAudioModuleWithInput, IAudioModuleWithOutput
{
    private readonly RingBuffer<byte> _ring;
    
    public int Size => _ring.Buffer.Length;
    public int Head => _ring.Head;
    public int Tail => _ring.Tail;

    public AudioBuffer(int size = 1024) {
        _ring = new RingBuffer<byte>(size);
    }

    protected override void InitAudioInternal(IServiceProvider services) {
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
