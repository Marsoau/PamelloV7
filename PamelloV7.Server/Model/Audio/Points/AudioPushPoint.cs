using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPushPoint : AudioPoint, IAudioPushPoint
{
    public Func<byte[], Task>? Process;
    
    public Task Push(byte[] audio) {
        if (Process is not null) return Process.Invoke(audio);
        
        return ((AudioPushPoint)FrontPoint!).Push(audio);
    }
}