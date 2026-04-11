using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordButtonsService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly IAssemblyTypeResolver _types;
    private readonly IEntityQueryService _peql;
    private readonly IPamelloUserRepository _users;
    
    private readonly UpdatableMessageService _messages;
    
    private List<Type> ButtonTypes { get; set; } = [];
    
    public DiscordButtonsService(IServiceProvider services) {
        _services = services;
        
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
        _peql = services.GetRequiredService<IEntityQueryService>();
        _users = services.GetRequiredService<IPamelloUserRepository>();
        
        _messages = services.GetRequiredService<UpdatableMessageService>();
    }

    public void Startup(IServiceProvider services) {
        var types = _types.GetInheritorsOf<DiscordButton>().ToList();
        
        ButtonTypes = types;
    }


    public async Task<TButton> GetAsync<TButton>(ButtonInteraction interaction)
        where TButton : DiscordButton
        => (TButton)await GetAsync(typeof(TButton), interaction);
    
    public async Task<DiscordButton> GetAsync(Type buttonType, ButtonInteraction interaction) {
        var scopeUser = await _users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), ServerConfig.Root.AllowUserCreation);
        if (scopeUser is null) throw new PamelloException("User could not be found/created");
        
        if (Activator.CreateInstance(buttonType) is not DiscordButton button)
            throw new PamelloException($"Discord button with custom id \"{interaction.Data.CustomId}\" was null on creation");
        
        var callSite = InteractionCallSite.FromString(interaction.Data.CustomId);
        var message = _messages.Get(callSite.Differentiator);
        if (message is null) throw new PamelloException($"Message for discord button with custom id \"{interaction.Data.CustomId}\" was not found");
        
        button.InitializeButton(message, interaction, _services, scopeUser);
        
        return button;
    }

    public static ButtonProperties GetProperties<TButton>(string customId)
        => GetProperties(typeof(TButton), customId);
    public static ButtonProperties GetProperties(Type buttonType, string customId) {
        var attribute = buttonType.GetCustomAttribute<DiscordButtonAttribute>();
        if (attribute is null) throw new PamelloException($"Discord button with custom id \"{buttonType.Name}\" does not have DiscordButton attribute");
        
        return new ButtonProperties(customId, attribute.Label, attribute.Style);
    }
}
