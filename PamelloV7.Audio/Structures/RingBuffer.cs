namespace PamelloV7.Server.Structures;

public class AwaitingOperation
{
    public int Size;
    public TaskCompletionSource<bool> Completion;
}

public class RingBuffer<TType> : IDisposable
{
    public int Tail { get; set; }
    public int Head { get; set; }
    
    public TType[] Buffer { get; set; }
    
    private List<AwaitingOperation> AwaitingWrites { get; set; }
    private List<AwaitingOperation> AwaitingReads { get; set; }

    // 1. Added a lock object to synchronize access to the lists
    private readonly object _lock = new();

    private bool _afterWrite;

    public RingBuffer(int size) {
        Buffer = new TType[size];
        
        AwaitingWrites = [];
        AwaitingReads = [];
        
        _afterWrite = false;
    }

    public void Write(TType value) {
        Buffer[Head] = value;
        
        if (++Head == Buffer.Length) Head = 0;
        _afterWrite = true;
    }
    
    public TType Read() {
        var value = Buffer[Tail];
        
        if (++Tail == Buffer.Length) Tail = 0;
        _afterWrite = false;
        
        return value;
    }

    public int Available()
    {
        if (Tail == Head) return _afterWrite ? 0 : Buffer.Length;
        var distance = Head - Tail;
        return distance < 0 ? -distance : Buffer.Length - distance;
    }
    public int Used()
    {
        if (Tail == Head) return _afterWrite ? Buffer.Length : 0;
        var distance = Head - Tail;
        return distance < 0 ? Buffer.Length - -distance : distance;
    }
    
    public bool WriteRange(TType[] values, bool wait, CancellationToken token = default)
    {
        var count = values.Length;
        if (count > Available())
        {
            if (!wait) return false;
            
            // 2. Use RunContinuationsAsynchronously to prevent deadlocks when SetResult is called inside a lock
            var operation = new AwaitingOperation {
                Size = count, 
                Completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)
            };
            token.Register(() => operation.Completion.TrySetResult(false));
            
            // 3. Lock adding to the list
            lock (_lock)
            {
                AwaitingWrites.Add(operation);
            }
            
            if (!operation.Completion.Task.Result) return false;
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
        _afterWrite = true;

        // Optimization: check count lightly before spawning task (optional but saves overhead)
        bool hasReaders;
        lock (_lock) hasReaders = AwaitingReads.Count > 0;

        if (hasReaders) _ = Task.Run(() => {
            // 4. Lock the iteration and removal
            lock (_lock)
            {
                foreach (var item in AwaitingReads)
                {
                    if (item.Size > Used()) continue;
            
                    AwaitingReads.Remove(item);
                    item.Completion.TrySetResult(true); // Changed to TrySetResult for safety
                    break;
                }
            }
        });

        return true;
    }
    
    public bool ReadRange(TType[] destination, bool wait, CancellationToken token = default)
    {
        var count = destination.Length;
        if (count > Used())
        {
            if (!wait) return false;
            
            // 2. Use RunContinuationsAsynchronously
            var operation = new AwaitingOperation {
                Size = count, 
                Completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)
            };
            token.Register(() => operation.Completion.TrySetResult(false));
            
            // 3. Lock adding to the list
            lock (_lock)
            {
                AwaitingReads.Add(operation);
            }
            
            if (!operation.Completion.Task.Result) return false;
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
        _afterWrite = false;
        
        bool hasWriters;
        lock (_lock) hasWriters = AwaitingWrites.Count > 0;

        if (hasWriters) _ = Task.Run(() => {
            // 4. Lock the iteration and removal
            lock (_lock)
            {
                foreach (var item in AwaitingWrites)
                {
                    if (item.Size > Available()) continue;
            
                    AwaitingWrites.Remove(item);
                    item.Completion.TrySetResult(true);
                    break;
                }
            }
        });
        
        return true;
    }

    public void Dispose()
    {
        // 5. Lock disposal cleanup
        lock (_lock)
        {
            foreach (var item in AwaitingWrites)
            {
                item.Completion.TrySetResult(false);
            }
            AwaitingWrites.Clear(); // Good practice to clear references

            foreach (var item in AwaitingReads)
            {
                item.Completion.TrySetResult(false);
            }
            AwaitingReads.Clear();
        }
    }
}