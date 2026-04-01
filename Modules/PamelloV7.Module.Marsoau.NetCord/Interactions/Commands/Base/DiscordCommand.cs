using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand : DiscordInteraction<SlashCommandInteraction>
{
    public async Task RespondAsync(string content) {
        if (!HasResponded) {
            await Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties() {
                Content = content,
                Flags = MessageFlags.Ephemeral
            }));
            
            HasResponded = true;
            
            return;
        }

        await Interaction.ModifyResponseAsync(options => {
            options.Content = content;
        });
    }
    
    public override Task RespondLoading() {
        if (HasResponded) return Task.CompletedTask;
        
        return RespondAsync("loading");
    }
}
