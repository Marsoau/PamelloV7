using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddShortInputAttribute<TValue> : AddInputAttribute<TValue> {
    public AddShortInputAttribute(string property, string label) : base(property, label, TextInputStyle.Short) { }
}
public class AddParagraphInputAttribute<TValue> : AddInputAttribute<TValue> {
    public AddParagraphInputAttribute(string property, string label) : base(property, label, TextInputStyle.Paragraph) { }
}

public class AddShortInputAttribute : AddInputAttribute<string> {
    public AddShortInputAttribute(string property, string label) : base(property, label, TextInputStyle.Short) { }
}
public class AddParagraphInputAttribute : AddInputAttribute<string> {
    public AddParagraphInputAttribute(string property, string label) : base(property, label, TextInputStyle.Paragraph) { }
}

public class AddInputAttribute : AddInputAttribute<string> {
    public AddInputAttribute(string property, string label, TextInputStyle style) : base(property, label, style) { }
}

public class AddInputAttribute<TValue> : AddModalPropertyAttribute<TextInputProperties, TValue>
{
    public string Label { get; }
    public TextInputStyle Style { get; }

    public AddInputAttribute(string property, string label, TextInputStyle style) : base(property) {
        Label = label;
        Style = style;
    }

    public override TextInputProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        var properties = new TextInputProperties(PropertyName, Style)
            .WithRequired(IsRequired);
        
        builder.Properties.AddComponents(new LabelProperties(Label, properties));
        
        return properties;
    }
    public override async Task<TValue> GetValueInAsync(ILabelComponent component, object? parentValue,
        IServiceProvider services, IPamelloUser scopeUser) {
        if (component is not TextInput input) return default!;

        return await PamelloBasicActions.InTypeFromStringAsync<TValue>(
            input.Value,
            "",
            services.GetRequiredService<IEntityQueryService>(),
            scopeUser
        );
    }
}