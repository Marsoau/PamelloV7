using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPullPoint : AudioPoint, IAudioPullPoint
{
    public Func<byte[], bool, Task<bool>>? OnRequest;

    public Task<bool> Pull(byte[] buffer, bool wait) {
        if (OnRequest is not null)
        {
            //Console.WriteLine("requesting custom pull on point");
            return OnRequest.Invoke(buffer, wait);
        }

        //Console.WriteLine("pulling from point");
        return ((AudioPullPoint)BackPoint!).Pull(buffer, wait);
    }
}