using System.Diagnostics;
using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public class AudioPushPoint : AudioPoint, IAudioPushPoint
{
    public Func<byte[], bool, CancellationToken, Task<bool>>? Process;

    public AudioPushPoint(IAudioModule parentModule) : base(parentModule) { }

    public async Task<bool> Push(byte[] audio, bool wait, CancellationToken token) {
        if (Process is not null)
        {
            try
            {
                return await Process.Invoke(audio, wait, token);
            }
            catch (Exception x)
            {
                Console.WriteLine($"Exception in custom push: {x}");
                await Task.Delay(1000, token);
                return false;
            }
        }
        
        if (FrontPoint is null) return await Task.FromResult(false);
        Debug.Assert(FrontPoint is AudioPushPoint, "Push point was connected to pull point");
        
        try
        {
            return await ((AudioPushPoint)FrontPoint!).Push(audio, wait, token);
        }
        catch (Exception x)
        {
            Console.WriteLine($"Exception in push: {x}");
            await Task.Delay(1000, token);
            return false;
        }
    }
}