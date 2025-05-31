namespace PamelloV7.Server.Model.Data;

public class AwaitingOperation
{
    public int Size;
    public TaskCompletionSource<bool> Completion;
}

public class CircularBuffer<TType> : IDisposable
{
    public int Tail { get; set; }
    public int Head { get; set; }
    
    public TType[] Buffer { get; set; }
    
    private List<AwaitingOperation> AwaitingWrites { get; set; }
    private List<AwaitingOperation> AwaitingReads { get; set; }

    private bool afterWrite;

    public CircularBuffer(int size) {
        Buffer = new TType[size];
        
        AwaitingWrites = [];
        AwaitingReads = [];
        
        afterWrite = false;
    }

    public void Write(TType value) {
        Buffer[Head] = value;
        
        if (++Head == Buffer.Length) Head = 0;
        afterWrite = true;
    }
    
    public TType Read() {
        var value = Buffer[Tail];
        
        if (++Tail == Buffer.Length) Tail = 0;
        afterWrite = false;
        
        return value;
    }

    public int Available()
    {
        if (Tail == Head) return afterWrite ? 0 : Buffer.Length;
        var distance = Head - Tail;
        return distance < 0 ? -distance : Buffer.Length - distance;
    }
    public int Used()
    {
        if (Tail == Head) return afterWrite ? Buffer.Length : 0;
        var distance = Head - Tail;
        return distance < 0 ? Buffer.Length - -distance : distance;
    }
    
    public async Task<bool> WriteRange(TType[] values, bool wait, CancellationToken token = default)
    {
        var count = values.Length;
        if (count > Available())
        {
            if (!wait) return false;
            
            var operation = new AwaitingOperation {Size = count, Completion = new TaskCompletionSource<bool>()};
            token.Register(() => operation.Completion.SetResult(false));
            
            AwaitingWrites.Add(operation);
            
            if (!await operation.Completion.Task) return false;
        }
        
        var capacity = Buffer.Length;

        var firstPart = Math.Min(count, capacity - Head);
        Array.Copy(values, 0, Buffer, Head, firstPart);

        var secondPart = count - firstPart;
        if (secondPart > 0)
        {
            Array.Copy(values, firstPart, Buffer, 0, secondPart);
        }

        Head = (Head + count) % capacity;
        afterWrite = true;

        if (AwaitingReads.Count > 0) _ = Task.Run(() => {
            foreach (var item in AwaitingReads)
            {
                if (item.Size > Used()) continue;
            
                AwaitingReads.Remove(item);
                item.Completion.SetResult(true);
                break;
            }
        });

        return true;
    }
    
    public async Task<bool> ReadRange(TType[] destination, bool wait, CancellationToken token = default)
    {
        var count = destination.Length;
        if (count > Used())
        {
            if (!wait) return false;
            
            var operation = new AwaitingOperation {Size = count, Completion = new TaskCompletionSource<bool>()};
            token.Register(() => operation.Completion.SetResult(false));
            
            AwaitingReads.Add(operation);
            
            if (!await operation.Completion.Task) return false;
        }
        
        var capacity = Buffer.Length;

        var firstPart = Math.Min(count, capacity - Tail);
        Array.Copy(Buffer, Tail, destination, 0, firstPart);

        var secondPart = count - firstPart;
        if (secondPart > 0)
        {
            Array.Copy(Buffer, 0, destination, firstPart, secondPart);
        }

        Tail = (Tail + count) % capacity;
        afterWrite = false;
        
        if (AwaitingWrites.Count > 0) _ = Task.Run(() => {
            foreach (var item in AwaitingWrites)
            {
                if (item.Size > Available()) continue;
            
                AwaitingWrites.Remove(item);
                item.Completion.SetResult(true);
                break;
            }
        });
        
        return true;
    }

    public void Dispose()
    {
        foreach (var item in AwaitingWrites)
        {
            item.Completion.SetResult(false);
        }
        foreach (var item in AwaitingReads)
        {
            item.Completion.SetResult(false);
        }
    }
}