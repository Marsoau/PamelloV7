using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPushPoint : AudioPoint, IAudioPushPoint
{
    public Func<byte[], bool, Task<bool>>? Process;

    public AudioPushPoint(IAudioModule parentModule) : base(parentModule) { }

    public Task<bool> Push(byte[] audio, bool wait) {
        if (Process is not null)
        {
            //Console.WriteLine("using custom push on point");
            return Process.Invoke(audio, wait);
        }
        
        if (FrontPoint is null) return Task.FromResult(false);
        
        //Console.WriteLine("pushing to point");
        return ((AudioPushPoint)FrontPoint!).Push(audio, wait);
    }
}