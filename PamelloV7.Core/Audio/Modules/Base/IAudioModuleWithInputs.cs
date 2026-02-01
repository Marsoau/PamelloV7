using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Core.Audio.Modules.Base;

public interface IAudioModuleWithInputs : IAudioModule
{
    bool IAudioModule.HasInputs => true;
    
    public List<IAudioPoint> Inputs { get; }
    
    public int MinInputs { get; }
    public int MaxInputs => Inputs.Capacity;

    public TAudioPoint AddInput<TAudioPoint>(Func<TAudioPoint> create) where TAudioPoint : IAudioPoint {
        var point = create();
        
        Inputs.Add(point);
        
        return point;
    }

    public void RemoveInput(int index) {
        if (index < 0 || index >= Inputs.Count) return;
        
        Inputs.RemoveAt(index);
    }
}
