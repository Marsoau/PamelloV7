using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Config;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Handlers;

public class InteractionHandler : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly DiscordClientService _clients;
    private readonly InteractionService _interactions;
    
    private readonly IPamelloUserRepository _users;
    
    public InteractionHandler(IServiceProvider services, DiscordClientService clients, InteractionService interactions)
    {
        _services = services;
        
        _clients = clients;
        _interactions = interactions;
        
        _users = services.GetRequiredService<IPamelloUserRepository>();
    }

    public async Task LoadAsync() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        
        var interactionTypes = typeResolver.GetInheritorsOf<DiscordCommand>();
        
        foreach (var interactionType in interactionTypes) {
            await _interactions.AddModuleAsync(interactionType, _services);
        }
        
        _clients.Main.InteractionCreated += OnInteractionCreated;
    }

    public async Task RegisterAsync() {
        if (DiscordConfig.Root.Commands.GlobalRegistration) {
            await _interactions.RegisterCommandsGloballyAsync();
        }
        else foreach (var guildId in DiscordConfig.Root.Commands.GuildsIds) {
            await _interactions.RegisterCommandsToGuildAsync(guildId);
        }
    }

    private async Task OnInteractionCreated(SocketInteraction interaction) {
        try {
            await ExecuteInteraction(interaction);
        }
        catch (Exception x) {
            Console.WriteLine("ERROR with interaction");
            Console.WriteLine($"Message: {x.Message}");
            Console.WriteLine($"More: {x}");
            if (interaction.GetOriginalResponseAsync() is not null) {
                await interaction.FollowupAsync("An error occured, check the console for more info", ephemeral: true);
            }
            else {
                await interaction.RespondAsync("An error occured, check the console for more info", ephemeral: true);
            }
        }
    }
    
    private async Task ExecuteInteraction(SocketInteraction interaction) {
        //await interaction.DeferAsync(true);

        var pamelloUser = _users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), true);
        if (pamelloUser is null) {
            await interaction.RespondAsync("Unexpected user error occurred");
            throw new Exception("Unexpected user error occurred");
        }

        var pamelloContext = new PamelloSocketInteractionContext(_services, interaction, pamelloUser);

        await _interactions.ExecuteCommandAsync(pamelloContext, _services);
    }
}
