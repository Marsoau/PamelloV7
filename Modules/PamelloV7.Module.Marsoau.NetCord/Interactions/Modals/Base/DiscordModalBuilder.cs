using System.Reflection;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

public abstract class DiscordModalBuilder : DiscordComponentBuilder
{
    private Type? _modalType;
    public Type ModalType => _modalType ?? throw new InvalidOperationException($"Modal type is not found on builder {GetType().FullName}");
    
    public string ModalId { get; set; } = null!;

    protected DiscordModalBuilder() {
        _modalType = GetType().DeclaringType!;
    }
    
    public ModalProperties Properties { get; protected set; } = null!;

    public virtual void InitializeBuilder(IServiceProvider services, string modalId, IPamelloUser scopeUser) {
        ModalId = modalId;
        
        InitializeActions(services, scopeUser);
        InitializeProperties();
    }

    private ModalProperties InitializeProperties() {
        var modalAttribute = ModalType.GetCustomAttribute<DiscordModalAttribute>();
        if (modalAttribute is null) throw new InvalidOperationException($"Modal type {ModalType.FullName} does not have DiscordModalAttribute");

        return Properties = new ModalProperties(ModalId, modalAttribute.Title);
    }
    
    public static Type GetFromModal(Type modalType) {
        var builderType = modalType.GetNestedTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(DiscordModalBuilder)));
        if (builderType is null) throw new PamelloException($"Modal {modalType.FullName} doesn't have DiscordModalBuilder");
        
        return builderType;
    }
}
