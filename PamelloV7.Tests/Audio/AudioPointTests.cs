using PamelloV7.Audio.Points;

namespace PamelloV7.Tests.Audio;

public class AudioPointTests
{
    private readonly CancellationTokenSource _cts = new();
    
    [Fact]
    public void BasicConnections() {
        var a = new AudioPoint(null);
        var b = new AudioPoint(null);

        Assert.ThrowsAny<Exception>(() => {
            a.ConnectedPoint = b;
        });
        Assert.ThrowsAny<Exception>(() => {
            b.ConnectedPoint = a;
        });

        a.ProcessAudio = (_, _, _) => true;
        a.ConnectedPoint = b;
        
        Assert.Equal(b, a.ConnectedPoint);
        Assert.Equal(a, b.ConnectedPoint);

        a.ConnectedPoint = null;
        
        Assert.Null(a.ConnectedPoint);
        Assert.Null(b.ConnectedPoint);
        
        b.ConnectedPoint = a;
        
        Assert.Equal(a, b.ConnectedPoint);
        Assert.Equal(b, a.ConnectedPoint);
    }
    
    [Fact]
    public void PassToProcess() {
        var a = new AudioPoint(null);
        
        var reached = false;
        
        a.ProcessAudio = (audio, _, _) => {
            reached = true;
            
            Assert.Equal([1, 2, 3, 4], audio);
            return true;
        };
        
        Assert.True(a.Pass([1, 2, 3, 4], true, _cts.Token));
        Assert.True(reached);
    }
    
    [Fact]
    public void PassToConnected() {
        var a = new AudioPoint(null);
        var b = new AudioPoint(null);

        var reached = false;

        a.ProcessAudio = (audio, _, _) => {
            reached = true;
            
            Assert.Equal([1, 2, 3, 4], audio);
            return true;
        };
        
        a.ConnectedPoint = b;
        
        Assert.True(b.Pass([1, 2, 3, 4], true, _cts.Token));
        Assert.True(reached);
    }
}
