using System.Diagnostics.CodeAnalysis;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Data;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioBuffer : IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithOutputs<AudioPullPoint>
{
    public int MinInputs => 1;
    public int MaxInputs => 1;

    public int MinOutputs => 1;
    public int MaxOutputs => 1;

    private readonly CircularBuffer<byte> _circle;
    public int Size => _circle.Buffer.Length;

    public AudioPushPoint Input;
    public AudioPullPoint Output;

    public bool IsDisposed { get; private set; }

    public AudioBuffer(int size) {
        _circle = new CircularBuffer<byte>(size);
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

    private Task<bool> OnRequest(byte[] buffer, bool wait) {
        return _circle.ReadRange(buffer, wait);
    }
    
    private Task<bool> Process(byte[] audio, bool wait) {
        return _circle.WriteRange(audio, wait);
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