using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddCheckBoxAttribute : AddModalPropertyAttribute<CheckboxProperties, bool>
{
    public string Label { get; }

    public AddCheckBoxAttribute(string property, string label, string name) : base(property) {
        Label = label;
    }
    
    public override CheckboxProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        var properties = new CheckboxProperties(PropertyName);
        
        builder.Properties.AddComponents(new LabelProperties(Label, properties));
        
        return properties;
    }
    public override async Task<bool> GetValueInAsync(ILabelComponent component, object? parentValue, IServiceProvider services, IPamelloUser scopeUser) {
        if (component is not Checkbox checkbox) return default!;
        
        return checkbox.Checked;
    }
}
