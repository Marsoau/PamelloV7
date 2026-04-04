using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddSelectOptionAttribute : AddModalPropertyAttribute<StringMenuSelectOptionProperties, bool> {
    public string Label { get; }
    public string Value { get; }
    
    public AddSelectOptionAttribute(string property, string label, string value) : base(property) {
        IsChild = true;
        
        Label = label;
        Value = value;
    }
    
    public override StringMenuSelectOptionProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        if (parentProperties is not StringMenuProperties menuProperties)
            throw new PamelloException("Parent properties must be StringMenuProperties");
        
        var properties = new StringMenuSelectOptionProperties(Label, Value);
        
        menuProperties.AddOptions(properties);
        
        return properties;
    }
    public override async Task<bool> GetValueInAsync(ILabelComponent component, object? parentValue, IServiceProvider services, IPamelloUser scopeUser) {
        if (parentValue is StringMenu menu) {
            return menu.SelectedValues?.Contains(Value) ?? false;
        }

        return false;
    }
}
