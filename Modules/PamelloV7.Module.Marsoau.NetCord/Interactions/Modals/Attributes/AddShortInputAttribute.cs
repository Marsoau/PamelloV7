using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddShortInputAttribute : AddModalPropertyAttribute<TextInputProperties, string>
{
    public string Label { get; }

    public AddShortInputAttribute(string property, string label) : base(property) {
        Label = label;
    }

    public override TextInputProperties AddPropertiesTo(DiscordModalBuilder builder) {
        var properties = new TextInputProperties(PropertyName, TextInputStyle.Short)
            .WithRequired(IsRequired);
        
        builder.Properties.AddComponents(new LabelProperties(Label, properties));
        
        return properties;
    }
    public override string GetValueIn(ILabelComponent component) {
        if (component is not TextInput input) return "";
        
        return input.Value;
    }
}
