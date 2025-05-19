using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPullPoint : AudioPoint, IAudioPullPoint
{
    public Func<byte[], Task>? OnRequest;
    public Func<byte[], Task<bool>>? OnTryRequest;

    public Task<bool> TryPull(byte[] buffer) {
        if (OnTryRequest is not null) return OnTryRequest.Invoke(buffer);
        
        return ((AudioPullPoint)BackPoint!).TryPull(buffer);
    }

    public Task Pull(byte[] buffer) {
        if (OnRequest is not null) return OnRequest.Invoke(buffer);
        
        return ((AudioPullPoint)BackPoint!).Pull(buffer);
    }
}