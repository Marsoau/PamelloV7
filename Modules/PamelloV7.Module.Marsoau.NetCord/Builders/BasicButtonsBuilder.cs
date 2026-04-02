using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class BasicButtonsBuilder : DiscordComponentBuilder
{
    public ActionRowProperties RefreshButton() {
        return new ActionRowProperties().AddComponents(
            new ButtonProperties("refresh", "Refresh", ButtonStyle.Secondary)
        );
    }
}
