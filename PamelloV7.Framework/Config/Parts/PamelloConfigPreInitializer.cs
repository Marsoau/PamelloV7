namespace PamelloV7.Framework.Config.Parts;

public class PamelloConfigPreInitializer
{
    public readonly PamelloConfigPart Part;
    
    public string PropertyPath { get; init; }
    public Type PropertyType { get; init; }

    private object? _value;
    public object? Value {
        get => _value;
        set {
            _value = value;
            IsPreInitialized = true;
        }
    }
    public bool IsPreInitialized { get; private set; }

    public PamelloConfigPreInitializer(PamelloConfigPart part, string propertyPath, Type propertyType) {
        Part = part;
        
        PropertyPath = propertyPath;
        PropertyType = propertyType;
    }

    public override string ToString() {
        return $"{PropertyPath}<{PropertyType}>";
    }
}
