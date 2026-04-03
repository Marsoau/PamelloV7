using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;

public interface IAddModalPropertyAttribute
{
    public string PropertyName { get; }
    
    public Type PropertiesType { get; }
    public Type ValueType { get; }
    
    public object AddPropertiesTo(DiscordModalBuilder builder);
    public object? GetValueIn(DiscordModal modal);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class AddModalPropertyAttribute<TProperties, TValue> : Attribute, IAddModalPropertyAttribute
    where TProperties : notnull
{
    public string PropertyName { get; set; }
    
    public Type PropertiesType => typeof(TProperties);
    public Type ValueType => typeof(TValue);

    protected AddModalPropertyAttribute(string property) {
        PropertyName = property;
    }
    
    public abstract TProperties AddPropertiesTo(DiscordModalBuilder builder);
    public abstract TValue GetValueIn(DiscordModal modal);
    
    object IAddModalPropertyAttribute.AddPropertiesTo(DiscordModalBuilder builder) => AddPropertiesTo(builder);
    object? IAddModalPropertyAttribute.GetValueIn(DiscordModal modal) => GetValueIn(modal);
}
