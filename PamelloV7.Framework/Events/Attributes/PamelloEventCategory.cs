using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PamelloEventCategory : Attribute
{
    public EEventCategory? Category { get; }
    
    private readonly string? _customCategory;
    public string CustomCategory => _customCategory ?? Category?.ToString() ?? "";

    public PamelloEventCategory(EEventCategory category) {
        Category = category;
        _customCategory = null;
    }
    public PamelloEventCategory(string customCategory) {
        Category = null;
        _customCategory = customCategory;
    }
}
