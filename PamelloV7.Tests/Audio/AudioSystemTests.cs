using PamelloV7.Core.Audio.Services;
using PamelloV7.Audio.Modules;
using PamelloV7.Audio.Services;

namespace PamelloV7.Tests.Audio;

public class AudioSystemTests
{
    private readonly IPamelloAudioSystem _audio;
    
    public AudioSystemTests() {
        _audio = new PamelloAudioSystem();
    }
    
    [Fact]
    public void BasicRegister() {
        var pump = _audio.RegisterModule(new AudioPump(2));
        
        Assert.NotNull(pump);
        Assert.NotNull(pump.Input);
        Assert.NotNull(pump.Output);
    }
}
