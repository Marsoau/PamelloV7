using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPushPoint : AudioPoint, IAudioPushPoint
{
    public Func<byte[], Task>? Process;
    public Func<byte[], Task<bool>>? TryProcess;

    public Task<bool> TryPush(byte[] audio) {
        if (TryProcess is not null) return TryProcess.Invoke(audio);
        
        return ((AudioPushPoint)FrontPoint!).TryPush(audio);
    }

    public Task Push(byte[] audio) {
        if (Process is not null) return Process.Invoke(audio);
        
        return ((AudioPushPoint)FrontPoint!).Push(audio);
    }
}