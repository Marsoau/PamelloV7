using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;
using PamelloV7.Server.Structures;

namespace PamelloV7.Server.Model.AudioOld.Modules.Basic;

public class AudioBuffer : IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinInputs => 1;
    public int MaxInputs => 1;

    public int MinOutputs => 1;
    public int MaxOutputs => 1;

    public AudioModel ParentModel { get; }

    private readonly RingBuffer<byte> _circle;
    public int Size => _circle.Buffer.Length;

    public AudioPushPoint Input;
    public AudioPullPoint Output;

    public bool IsDisposed { get; private set; }

    public AudioBuffer(AudioModel parentModel, int size) {
        _circle = new RingBuffer<byte>(size);
        
        ParentModel = parentModel;
    }
    
    public AudioPushPoint CreateInput() {
        Input = new AudioPushPoint(this);
        
        Input.Process = Process;

        return Input;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint(this);
        
        Output.OnRequest = OnRequest;
        
        return Output;
    }

    private Task<bool> OnRequest(byte[] buffer, bool wait, CancellationToken token) {
        return Task.FromResult(false); //_circle.ReadRange(buffer, wait, token);
    }
    
    private Task<bool> Process(byte[] audio, bool wait, CancellationToken token) {
        return Task.FromResult(false); //_circle.WriteRange(audio, wait, token);
    }

    public void InitModule() {
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        Input.Dispose();
        Output.Dispose();
        _circle.Dispose();
    }
}