using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioPump : AudioModule<AudioPullPoint, AudioPushPoint>
{
    protected override int MinInputs => 1;
    protected override int MaxInputs => 1;
    protected override int MinOutputs => 1;
    protected override int MaxOutputs => 1;
    
    public AudioPullPoint Input;
    public AudioPushPoint Output;
    
    public override AudioPullPoint CreateInput() {
        Input = base.CreateInput();
        
        return Input;
    }

    public override AudioPushPoint CreateOutput() {
        Output = base.CreateOutput();
        
        return Output;
    }

    public override void Init() {
        base.Init();
    }
    
    public async Task Start() {
        var pair = new byte[2];
        
        while (true) {
            await Input.Pull(pair);
            await Output.Push(pair);
        }
    }
}