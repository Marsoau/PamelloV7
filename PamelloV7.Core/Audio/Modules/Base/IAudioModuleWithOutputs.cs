using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Core.Audio.Modules.Base;

public interface IAudioModuleWithOutputs : IAudioModule
{
    bool IAudioModule.HasOutputs => true;
    
    public List<IAudioPoint> Outputs { get; }
    
    public int MinOutputs { get; }
    public int MaxOutputs => Outputs.Capacity;
    
    public TAudioPoint AddOutput<TAudioPoint>(Func<TAudioPoint> create) where TAudioPoint : IAudioPoint {
        var point = create();
        
        Outputs.Add(point);
        
        return point;
    }

    public void RemoveOutputs(int index) {
        if (index < 0 || index >= Outputs.Count) return;
        
        Outputs.RemoveAt(index);
    }
}
