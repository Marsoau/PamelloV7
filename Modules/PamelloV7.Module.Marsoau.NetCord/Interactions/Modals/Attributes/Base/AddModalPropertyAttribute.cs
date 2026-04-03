using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;

public interface IAddModalPropertyAttribute
{
    public string PropertyName { get; }
    
    public Type PropertiesType { get; }
    public Type ValueType { get; }
    
    public object AddPropertiesTo(DiscordModalBuilder builder);
    public object? GetValueIn(ILabelComponent component);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class AddModalPropertyAttribute<TProperties, TValue> : Attribute, IAddModalPropertyAttribute
    where TProperties : notnull
{
    public string PropertyName { get; }
    public bool IsRequired { get; }
    
    public Type PropertiesType => typeof(TProperties);
    public Type ValueType => typeof(TValue);

    protected AddModalPropertyAttribute(string property) {
        IsRequired = property.EndsWith('*');
        PropertyName = IsRequired ? property[..^1] : property;
    }
    
    public abstract TProperties AddPropertiesTo(DiscordModalBuilder builder);
    public abstract TValue GetValueIn(ILabelComponent component);
    
    object IAddModalPropertyAttribute.AddPropertiesTo(DiscordModalBuilder builder) => AddPropertiesTo(builder);
    object? IAddModalPropertyAttribute.GetValueIn(ILabelComponent component) => GetValueIn(component);
}
