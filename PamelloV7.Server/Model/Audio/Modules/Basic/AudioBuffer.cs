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
        Input.TryProcess = TryProcess;

        return Input;
    }

    public AudioPullPoint CreateOutput() {
        Output = new AudioPullPoint();
        
        Output.OnRequest = OnRequest;
        Output.OnTryRequest = OnTryRequest;
        
        return Output;
    }

    private async Task OnRequest(byte[] buffer) {
        await _circle.ReadRange(buffer, true);
    }
    private async Task<bool> OnTryRequest(byte[] buffer) {
        return await _circle.ReadRange(buffer, false);
    }
    
    private async Task Process(byte[] audio) {
        await _circle.WriteRange(audio, true);
    }
    private async Task<bool> TryProcess(byte[] audio) {
        return await _circle.WriteRange(audio, false);
    }

    public void InitModule() {
    }
}