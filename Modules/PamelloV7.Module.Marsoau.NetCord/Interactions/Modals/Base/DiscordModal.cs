using System.Reflection;
using NetCord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

public abstract class DiscordModal : DiscordInteraction<ModalInteraction>
{
    private Type? _builderType;
    public Type BuilderType => _builderType ?? throw new InvalidOperationException($"Modal type is not found on builder {GetType().FullName}");

    public string ModalId { get; set; } = null!;
    
    protected DiscordModal() {
        _builderType = GetType().GetNestedTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(DiscordModalBuilder)));
    }

    public virtual void InitializeModal(string modalId, Interaction interaction, IServiceProvider services, IPamelloUser scopeUser) {
        base.InitializeInteraction(interaction, services, scopeUser);
        
        if (interaction is not ModalInteraction modalInteraction)
            throw new PamelloException($"Interaction is not a modal interaction");
        
        object? parentValue = null;

        IAddModalPropertyAttribute[] attributes = [
            ..GetType().GetCustomAttributes().OfType<IAddModalPropertyAttribute>(),
            ..BuilderType.GetCustomAttributes().OfType<IAddModalPropertyAttribute>()
        ];

        var tasks = modalInteraction.Data.Components
            .OfType<Label>()
            .Select(label => SetValueForComponent(label.Component));
        
        Task.WhenAll(tasks).Wait();
        
        return;

        async Task SetValueForComponent(ILabelComponent component) {
            var customIdProperty = component.GetType().GetProperty("CustomId");
            var customId = customIdProperty?.GetValue(component);
            
            if (customId is not string customIdString) throw new PamelloException($"CustomId not found in {component}");
            
            var attribute = attributes.FirstOrDefault(a => a.PropertyName == customIdString);
            if (attribute is null) throw new PamelloException($"Attribute for name of custom id \"{customIdString}\" not found");

            var value = await attribute.GetValueInAsync(component, parentValue, Services, ScopeUser);
            
            if (!attribute.IsChild) {
                parentValue = value;
            }

            var property = GetType().GetProperty(attribute.PropertyName);
            property?.SetValue(this, value);
        }
    }

    public override string GetCallSiteInteractionDifferentiator() {
        throw new PamelloException("Cannot create Differentiator in DiscordModal");
    }
}
