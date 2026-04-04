using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;

public interface IAddModalPropertyAttribute
{
    public bool IsChild { get; }
    public string PropertyName { get; }
    public bool IsRequired { get; }
    
    public Type PropertiesType { get; }
    public Type ValueType { get; }
    
    public object AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties);
    public Task<object?> GetValueInAsync(ILabelComponent component, object? parentValue, IServiceProvider services, IPamelloUser scopeUser);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class AddModalPropertyAttribute<TProperties, TValue> : Attribute, IAddModalPropertyAttribute
    where TProperties : notnull
{
    public bool IsChild { get; protected set; }
    public string PropertyName { get; }
    public bool IsRequired { get; }
    
    public Type PropertiesType => typeof(TProperties);
    public Type ValueType => typeof(TValue);

    protected AddModalPropertyAttribute(string property) {
        IsRequired = property.EndsWith('*');
        PropertyName = IsRequired ? property[..^1] : property;
    }
    
    public abstract TProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties);

    public abstract Task<TValue> GetValueInAsync(ILabelComponent component, object? parentValue, IServiceProvider services, IPamelloUser scopeUser);
    
    object IAddModalPropertyAttribute.AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) => AddPropertiesTo(builder, parentProperties);
    async Task<object?> IAddModalPropertyAttribute.GetValueInAsync(ILabelComponent component, object? parentValue,
        IServiceProvider services, IPamelloUser scopeUser)
        => await GetValueInAsync(component, parentValue, services, scopeUser);
}
