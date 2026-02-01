using PamelloV7.Server.Structures;
using Xunit.Abstractions;

namespace PamelloV7.Tests.Data;

public class RingBufferTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RingBufferTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private void WriteHeadTail(RingBuffer<int> buffer)
    {
        _testOutputHelper.WriteLine($"{buffer.Tail}:{buffer.Head}");
    }
    private void Write(RingBuffer<int> buffer)
    {
        _testOutputHelper.WriteLine("---");
        for (int i = 0; i < buffer.Buffer.Length; i++)
        {
            if (i == buffer.Tail && i == buffer.Head)
            {
                _testOutputHelper.WriteLine($"^ {buffer.Buffer[i]}");
                continue;
            }
            if (i != buffer.Tail && i != buffer.Head)
            {
                _testOutputHelper.WriteLine($"| {buffer.Buffer[i]}");
                continue;
            }
            if (i == buffer.Tail)
                _testOutputHelper.WriteLine($"< {buffer.Buffer[i]}");
            if (i == buffer.Head)
                _testOutputHelper.WriteLine($"> {buffer.Buffer[i]}");
        }
        _testOutputHelper.WriteLine("---");
    }

    [Fact]
    public void IsRightOnCreation()
    {
        var buffer = new RingBuffer<int>(10);
        
        for (var i = 0; i < 10; i++)
        {
            Assert.Equal(0, buffer.Read());
        }
        
        Write(buffer);
        
        Assert.Equal(0, buffer.Tail);
        Assert.Equal(0, buffer.Head);
        
        Assert.Equal(10, buffer.Available());
        Assert.Equal(0, buffer.Used());
    }
    
    [Fact]
    public void WriteReadByOne()
    {
        var buffer = new RingBuffer<int>(5);
        
        WriteHeadTail(buffer);
        
        buffer.Write(5);
        buffer.Write(2);
        buffer.Write(8);
        
        Write(buffer);
        
        WriteHeadTail(buffer);
        Assert.Equal(0, buffer.Tail);
        Assert.Equal(3, buffer.Head);
        
        Assert.Equal(5, buffer.Read());
        Assert.Equal(2, buffer.Read());
        Assert.Equal(8, buffer.Read());
        
        WriteHeadTail(buffer);
        Assert.Equal(3, buffer.Tail);
        Assert.Equal(3, buffer.Head);
    }

    [Fact]
    public void WriteReadRange()
    {
        var buffer = new RingBuffer<int>(5);

        buffer.WriteRange([5, 2, 8], true);
        
        Write(buffer);
        WriteHeadTail(buffer);
        Assert.Equal(0, buffer.Tail);
        Assert.Equal(3, buffer.Head);

        var read = new int[3];
        buffer.ReadRange(read, true);
        
        WriteHeadTail(buffer);
        Assert.Equal([5, 2, 8], read);
        Assert.Equal(3, buffer.Tail);
        Assert.Equal(3, buffer.Head);
    }

    [Fact]
    public void WriteReadWithOverflow()
    {
        var buffer = new RingBuffer<int>(5);
        
        buffer.WriteRange([5, 2, 8], true);
        
        Write(buffer);
        Assert.Equal(2, buffer.Available());
        Assert.Equal(3, buffer.Used());

        var writing = Task.Run(() => buffer.WriteRange([1, 2, 3], true));
        
        var read = new int[3];
        buffer.ReadRange(read, true);
        
        Write(buffer);
        Assert.Equal(5, buffer.Available());
        Assert.Equal(0, buffer.Used());

        writing.Wait();
        
        Write(buffer);
        Assert.Equal(2, buffer.Available());
        Assert.Equal(3, buffer.Used());
        
        read = new int[5];
        var reading = Task.Run(() => buffer.ReadRange(read, true));

        buffer.WriteRange([4, 5], true);

        reading.Wait();
        
        Write(buffer);
        Assert.Equal(5, buffer.Available());
        Assert.Equal(0, buffer.Used());
    }
}