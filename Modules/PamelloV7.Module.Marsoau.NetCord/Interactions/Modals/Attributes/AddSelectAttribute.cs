using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddSelectAttribute : AddSelectAttribute<StringMenu>
{
    public AddSelectAttribute(string property, string label) : base(property, label) { }
}

public class AddSelectAttribute<TValue> : AddModalPropertyAttribute<StringMenuProperties, TValue> {
    public string Label { get; }

    public AddSelectAttribute(string property, string label) : base(property) {
        Label = label;
    }
    
    public override StringMenuProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        var properties = new StringMenuProperties(PropertyName)
            .WithRequired(IsRequired);
        
        builder.Properties.AddComponents(new LabelProperties(Label, properties));
        
        return properties;
    }
    public override async Task<TValue> GetValueInAsync(ILabelComponent component, object? parentValue,
        IServiceProvider services, IPamelloUser scopeUser) {
        if (component is not StringMenu menu) return default!;
        
        if (typeof(TValue) == typeof(StringMenu)) {
            return (TValue)component;
        }

        var value = menu.SelectedValues?.FirstOrDefault();
        if (value is null) return default!;
        
        return await PamelloBasicActions.InTypeFromStringAsync<TValue>(
            value,
            "",
            services.GetRequiredService<IEntityQueryService>(),
            scopeUser
        );
    }
}
