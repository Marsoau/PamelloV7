using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Core.Events.Attributes;

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
