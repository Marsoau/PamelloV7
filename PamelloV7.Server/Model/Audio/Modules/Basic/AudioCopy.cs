using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioCopy : AudioModule<AudioPushPoint, AudioPushPoint>
{
    protected override int MinInputs => 1;
    protected override int MaxInputs => 1;
    
    protected override int MinOutputs => 0;
    protected override int MaxOutputs => 10;

    public AudioPushPoint Input;
    
    public override AudioPushPoint CreateInput() {
        Input = base.CreateInput();

        Input.Process = ProcessInput;

        return Input;
    }

    private Task ProcessInput(byte[] audio) {
        return Task.WhenAll(Outputs.Select(o => o.Push(audio)));
    }
}