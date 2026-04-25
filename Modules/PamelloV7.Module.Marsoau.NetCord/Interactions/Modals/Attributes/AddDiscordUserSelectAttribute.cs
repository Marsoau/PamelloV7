using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddDiscordUserSelectAttribute : AddModalPropertyAttribute<UserMenuProperties, global::NetCord.User>
{
    public string Label { get; }

    public AddDiscordUserSelectAttribute(string property, string label) : base(property) {
        Label = label;
    }
    
    public override UserMenuProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        var properties = new UserMenuProperties(PropertyName)
            .WithRequired(IsRequired);
        
        builder.Properties.AddComponents(new LabelProperties(Label, properties));
        
        return properties;
    }
    public override Task<global::NetCord.User> GetValueInAsync(ILabelComponent component, object? parentValue, IServiceProvider services, IPamelloUser scopeUser) {
        throw new NotImplementedException();
    }
}
