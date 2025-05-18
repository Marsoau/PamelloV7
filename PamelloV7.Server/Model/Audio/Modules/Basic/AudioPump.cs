using System.Runtime.CompilerServices;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules.Basic;

public class AudioPump : IAudioModuleWithInputs<AudioPullPoint>, IAudioModuleWithOutputs<AudioPushPoint>
{
    public int MinInputs => 1;
    public int MaxInputs => 1;
    
    public int MinOutputs => 1;
    public int MaxOutputs => 1;
    
    public AudioPullPoint Input;
    public AudioPushPoint Output;

    public int ChunkSize;

    public AudioPump() : this(2) {}
    public AudioPump(int chunkSize) {
        ChunkSize = chunkSize;
    }
    
    public AudioPullPoint CreateInput() {
        Input = new AudioPullPoint();
        
        return Input;
    }

    public AudioPushPoint CreateOutput() {
        Output = new AudioPushPoint();
        
        return Output;
    }
    
    public async Task Start() {
        var pair = new byte[ChunkSize];
        
        while (true) {
            await Input.Pull(pair);
            await Output.Push(pair);
        }
    }

    public void InitModule() {
    }
}