using System.Diagnostics.CodeAnalysis;
using PamelloV7.Server.Data;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

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

    public AudioBuffer(int size) {
        _circle = new CircularBuffer<byte>(size);
    }
    
    public AudioPushPoint CreateInput() {
        Input = new AudioPushPoint();
        
        Input.Process = Process;

        return Input;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint();
        
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
}