using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public record DiscordModalDescriptor(
    DiscordModalAttribute Attribute,
    Type ModalType,
    Type BuilderType
);

public class DiscordModalsService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly IPamelloUserRepository _users;
    
    private readonly IAssemblyTypeResolver _types;
    
    public List<DiscordModalDescriptor> ModalsDescriptors { get; private set; } = [];

    public DiscordModalsService(IServiceProvider services) {
        _services = services;
        
        _users = services.GetRequiredService<IPamelloUserRepository>();       
        
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
    }
    
    public void Startup(IServiceProvider services) {
        var types = _types.GetInheritorsOf<DiscordModal>().ToList();
        
        ModalsDescriptors = types.Select(modalType => new DiscordModalDescriptor(
            modalType.GetCustomAttribute<DiscordModalAttribute>()
                ?? throw new PamelloException($"Modal {modalType.FullName} doesn't have DiscordModalAttribute"),
            modalType,
            DiscordModalBuilder.GetTypeFromModal(modalType)
        )).ToList();

        Output.Write("Discord modals:");
        foreach (var modal in ModalsDescriptors) {
            Output.Write($"| {modal.Attribute.Title} - {modal.ModalType.FullName}");
            Output.Write($"|   {modal.BuilderType.FullName}");
        }
    }

    public async Task<DiscordModalBuilder> GetBuilder(Type builderType, ButtonInteraction interaction) {
        var scopeUser = await _users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), ServerConfig.Root.AllowUserCreation);
        if (scopeUser is null) throw new PamelloException("User could not be found/created");
        
        var descriptor = ModalsDescriptors.FirstOrDefault(d => d.BuilderType == builderType);
        if (descriptor is null) throw new PamelloException($"Could not find builder {builderType.FullName}");
        
        var callSite = InteractionCallSite.FromString(interaction.Data.CustomId);

        if (Activator.CreateInstance(descriptor.BuilderType) is not DiscordModalBuilder builder)
            throw new PamelloException($"Could not create builder {builderType.FullName}");
        
        builder.InitializeModalBuilder(callSite, _services, scopeUser);
        
        return builder;
    }
    
    public async Task<DiscordModal> GetModal(Type modalType, ModalInteraction interaction) {
        var scopeUser = await _users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), ServerConfig.Root.AllowUserCreation);
        if (scopeUser is null) throw new PamelloException("User could not be found/created");
        
        var descriptor = ModalsDescriptors.FirstOrDefault(d => d.ModalType == modalType);
        if (descriptor is null) throw new PamelloException($"Could not find modal {modalType.FullName}");

        if (Activator.CreateInstance(descriptor.ModalType) is not DiscordModal modal)
            throw new PamelloException($"Could not create modal {modalType.FullName}");
        
        modal.InitializeModal(interaction.Data.CustomId, interaction, _services, scopeUser);
        
        return modal;   
    }
}
