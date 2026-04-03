using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddShortInputAttribute : AddShortInputAttribute<string> {
    public AddShortInputAttribute(string property, string label) : base(property, label) { }
}

public class AddShortInputAttribute<TValue> : AddModalPropertyAttribute<TextInputProperties, TValue>
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
    public override async Task<TValue> GetValueInAsync(ILabelComponent component, IServiceProvider services, IPamelloUser scopeUser) {
        if (component is not TextInput input) return default!;

        return await PamelloBasicActions.InTypeFromStringAsync<TValue>(
            input.Value,
            "",
            services.GetRequiredService<IEntityQueryService>(),
            scopeUser
        );
    }
}
