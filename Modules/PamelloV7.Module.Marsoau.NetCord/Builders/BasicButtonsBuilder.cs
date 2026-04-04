using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class BasicButtonsBuilder : DiscordComponentBuilder
{
    public ActionRowProperties RefreshButtonRow() {
        return new ActionRowProperties().AddComponents(
            Button<RefreshButton>()
        );
    }
    
    public ActionRowProperties? PageButtons(bool displayPrev, bool displayNext) {
        if (!displayPrev && !displayNext) return null;
        
        var actionRow = new ActionRowProperties();
        
        if (displayPrev) {
            actionRow.AddComponents(
                Button<PrevButton>()
            );
        }
        if (displayNext) {
            actionRow.AddComponents(
                Button<NextButton>()
            );
        }

        return actionRow;
    }
}
