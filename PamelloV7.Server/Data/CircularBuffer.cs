namespace PamelloV7.Server.Data;

public class CircularBuffer<TType>
{
    public int Tail { get; set; }
    public int Head { get; set; }
    
    public TType[] Buffer { get; set; }

    public CircularBuffer(int size) {
        Buffer = new TType[size];
    }

    public void Write(TType value) {
        Buffer[Head] = value;
        
        if (++Head == Buffer.Length) Head = 0;
    }
    
    public TType Read() {
        var value = Buffer[Tail];
        
        if (++Tail == Buffer.Length) Tail = 0;
        
        return value;
    }

    public void WritePair(TType[] pair) {
        Write(pair[0]);
        Write(pair[1]);
    }
    public void ReadPair(TType[] pair) {
        pair[0] = Read();
        pair[1] = Read();
    }
    
    public void WriteRange(TType[] values) {
        var partSize = values.Length < Buffer.Length - Head ?
            Buffer.Length - Head :
            values.Length;

        for (var i = 0; i < partSize; i++) {
            Buffer[Head + i] = values[i];
        }
        
        Head = 0;
        
        if (partSize < values.Length) {
            for (var i = 0; i < values.Length - partSize; i++) {
                Buffer[Head + i] = values[partSize + i];
            }
        }

        Head = values.Length - partSize;
    }
}