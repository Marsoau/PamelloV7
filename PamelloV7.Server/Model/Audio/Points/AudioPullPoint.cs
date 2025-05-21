using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPullPoint : AudioPoint, IAudioPullPoint
{
    public Func<byte[], bool, Task<bool>>? OnRequest;

    public AudioPullPoint(IAudioModule parentModule) : base(parentModule) { }

    public Task<bool> Pull(byte[] buffer, bool wait) {
        if (OnRequest is not null)
        {
            //Console.WriteLine("requesting custom pull on point");
            return OnRequest.Invoke(buffer, wait);
        }
        
        if (BackPoint is null) return Task.FromResult(false);
        Debug.Assert(BackPoint is AudioPullPoint, "Pull point was connected to push point");

        //Console.WriteLine("pulling from point");
        return ((AudioPullPoint)BackPoint!).Pull(buffer, wait);
    }
}