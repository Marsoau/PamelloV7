using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Messages;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands;

[Map]
public class GeneralButtons : DiscordCommand
{
    [ComponentInteraction("page-next")]
    public async Task PageNextButton() {
        if (Context.Interaction is not SocketMessageComponent component) {
            await ReleaseInteractionAsync();
            return;
        }
        
        var messageService = Services.GetRequiredService<UpdatableMessageKiller>();
        var message = messageService.Get(component.Message.Id);

        if (message is not UpdatablePageMessage pageMessage) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await pageMessage.SetPage(pageMessage.Page + 1);
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("page-prev")]
    public async Task PagePrevButton() {
        if (Context.Interaction is not SocketMessageComponent component) {
            await ReleaseInteractionAsync();
            return;
        }
        
        var messageService = Services.GetRequiredService<UpdatableMessageKiller>();
        var message = messageService.Get(component.Message.Id);

        if (message is not UpdatablePageMessage pageMessage) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await pageMessage.SetPage(pageMessage.Page - 1);
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("refresh")]
    public async Task RefreshButton() {
        if (Context.Interaction is not SocketMessageComponent component) {
            await ReleaseInteractionAsync();
            return;
        }
        
        var messageService = Services.GetRequiredService<UpdatableMessageKiller>();
        var message = messageService.Get(component.Message.Id);
        if (message is null) {
            await ReleaseInteractionAsync();
            return;
        }

        await message.Refresh();
        await ReleaseInteractionAsync();
    }

    [ComponentInteraction("pamello-command:*")]
    public async Task PamelloCommand(string commandPath) {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        
        await commands.ExecutePathAsync(commandPath, ScopeUser);
        
        await ReleaseInteractionAsync();
    }
}
