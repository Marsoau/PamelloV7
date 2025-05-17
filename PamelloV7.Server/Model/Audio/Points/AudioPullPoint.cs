using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPullPoint : AudioPoint, IAudioPullPoint
{
    public Func<byte[], Task>? OnRequest;
    
    public Task Pull(byte[] buffer) {
        if (OnRequest is not null) return OnRequest.Invoke(buffer);
        
        return ((AudioPullPoint)BackPoint!).Pull(buffer);
    }
}