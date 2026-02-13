using PamelloV7.Audio.Modules;
using PamelloV7.Audio.Points;
using PamelloV7.Audio.Services;
using PamelloV7.Core.Audio.Services;

namespace PamelloV7.Tests.Audio;

public class AudioBufferTests
{
    private readonly IPamelloAudioSystem _audio;
    
    public AudioBufferTests() {
        _audio = new PamelloAudioSystem();
    }
    
    [Fact]
    public void BasicTransfer() {
        var buffer = _audio.RegisterModule(new AudioBuffer(5));
        
        Assert.Equal(0, buffer.Head);
        Assert.Equal(0, buffer.Tail);
        
        Assert.True(buffer.Input.Pass([1, 2, 3, 4], false, CancellationToken.None));
        
        Assert.Equal(4, buffer.Head);
        Assert.Equal(0, buffer.Tail);

        var bytes = new byte[4];
        
        Assert.False(buffer.Input.Pass([5, 6, 8], false, CancellationToken.None));
        Assert.True(buffer.Output.Pass(bytes, false, CancellationToken.None));
        
        Assert.Equal(4, buffer.Head);
        Assert.Equal(4, buffer.Tail);
        
        Assert.Equal([1, 2, 3, 4], bytes);
        
        Assert.True(buffer.Input.Pass([5, 6, 8], false, CancellationToken.None));
        
        bytes = new byte[3];
        
        Assert.True(buffer.Output.Pass(bytes, false, CancellationToken.None));
        Assert.Equal([5, 6, 8], bytes);
    }
}
