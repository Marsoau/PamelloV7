using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Services;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord.Handlers;

public class InteractionHandler : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly DiscordClientService _clients;
    private readonly InteractionService _interactions;
    
    private readonly DiscordComponentBuildersService _components;
    
    private readonly IPamelloUserRepository _users;
    
    public InteractionHandler(IServiceProvider services, DiscordClientService clients, InteractionService interactions)
    {
        _services = services;
        
        _clients = clients;
        _interactions = interactions;
        
        _components = services.GetRequiredService<DiscordComponentBuildersService>();
        
        _users = services.GetRequiredService<IPamelloUserRepository>();
    }

    public async Task LoadAsync() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();

        var maps = typeResolver.GetWithAttribute<MapAttribute>().ToArray();

        foreach (var map in maps) {
            await _interactions.AddModuleAsync(map, _services);
        }
        
        _clients.Main.InteractionCreated += OnInteractionCreated;
        _interactions.SlashCommandExecuted += InteractionsOnSlashCommandExecuted;
    }

    private async Task InteractionsOnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result) {
        if (result.IsSuccess || result.Error != InteractionCommandError.Exception) return;
        if (result is not ExecuteResult executeResult) return;
        if (context is not PamelloSocketInteractionContext pamelloContext) return;
        if (executeResult.Exception?.InnerException is not PamelloException exception) {
            Console.WriteLine($"Exception in interaction: {command.Name}\n{executeResult.Exception}");
            return;
        }
        
        if (context.Interaction.HasResponded) {
            Console.WriteLine("Folow ephemerally");
            await context.Interaction.FollowupAsync(components: _components.GetBuilder<BasicComponentsBuilder>(pamelloContext).Info("Error", exception.Message).Build(), ephemeral: true);
        }
        else {
            Console.WriteLine("Responding ephemerally");
            await context.Interaction.RespondAsync(components: _components.GetBuilder<BasicComponentsBuilder>(pamelloContext).Info("Error", exception.Message).Build(), ephemeral: true);
        }
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
            Console.WriteLine("Executing interaction");
            await ExecuteInteraction(interaction);
            Console.WriteLine("Interaction executed");
        }
        catch (PamelloException x) {
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
