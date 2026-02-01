using PamelloV7.Core.Audio.Services;
using PamelloV7.Audio.Modules;
using PamelloV7.Audio.Points;
using PamelloV7.Audio.Services;

namespace PamelloV7.Tests.Audio;

public class AudioPumpTests
{
    private readonly IPamelloAudioSystem _audio;
    
    public AudioPumpTests() {
        _audio = new PamelloAudioSystem();
    }
    
    [Fact]
    public void BasicTransfer() {
        var sourcePoint = new AudioPoint(null);
        var destinationPoint = new AudioPoint(null);

        var sourceRun = false;
        var destinationRun = false;
        
        sourcePoint.ProcessAudio = (audio) => {
            Assert.All(audio, a => Assert.Equal(0, a));

            for (var i = 0; i < audio.Length; i++) {
                audio[i] = 3;
            }
            
            return sourceRun = true;
        };
        
        destinationPoint.ProcessAudio = (audio) => {
            Assert.All(audio, a => Assert.Equal(3, a));

            return destinationRun = true;
        };
        
        var pump = _audio.Register(new AudioPump(10));
        
        pump.Input.ConnectedPoint = sourcePoint;
        pump.Output.ConnectedPoint = destinationPoint;
        
        pump.Pump();
        
        Assert.True(sourceRun);
        Assert.True(destinationRun);
    }
}
