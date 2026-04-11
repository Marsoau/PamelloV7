using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Buttons;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class BasicButtonsBuilder : DiscordComponentBuilder
{
    public ActionRowProperties RefreshButtonRow() {
        return new ActionRowProperties().AddComponents(
            Button<RefreshButton>()
        );
    }
    
    public ActionRowProperties? PageButtons(int page, int pageSize, int totalItems) {
        var totalPages = totalItems / pageSize + (totalItems % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;
        
        var displayPrev = page != 0;
        var displayNext = page < totalPages - 1;
        
        if (!displayPrev && !displayNext) return null;
        
        var actionRow = new ActionRowProperties();
        
        if (displayPrev || totalPages > 3) {
            actionRow.AddComponents(
                Button<PrevPageButton>().WithDisabled(!displayPrev)
            );
        }
        if (totalPages > 3) {
            actionRow.AddComponents(
                ModalButton<SelectPageButtonModal>("Page", ButtonStyle.Secondary, [page, pageSize, totalItems])
            );
        }
        if (displayNext || totalPages > 3) {
            actionRow.AddComponents(
                Button<NextPageButton>().WithDisabled(!displayNext)
            );
        }

        return actionRow;
    }
}
