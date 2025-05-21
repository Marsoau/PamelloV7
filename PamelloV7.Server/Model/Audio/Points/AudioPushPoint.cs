using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPushPoint : AudioPoint, IAudioPushPoint
{
    public Func<byte[], bool, Task<bool>>? Process;

    public AudioPushPoint(IAudioModule parentModule) : base(parentModule) { }

    public async Task<bool> Push(byte[] audio, bool wait) {
        if (Process is not null)
        {
            //Console.WriteLine("using custom push on point");
            if (await Process.Invoke(audio, wait)) return true;

            // Console.WriteLine("was push false");
            return false;
        }
        
        if (FrontPoint is null) return await Task.FromResult(false);
        Debug.Assert(FrontPoint is AudioPushPoint, "Push point was connected to pull point");
        
        //Console.WriteLine("pushing to point");
        return await ((AudioPushPoint)FrontPoint!).Push(audio, wait);
    }
}