using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class PlayerPauseToggleBuilder : DiscordComponentBuilder
{
    public ComponentContainerProperties Container() {
        var container = new ComponentContainerProperties().AddComponents(
            new TextDisplayProperties(
                $"### Pause Toggle{(SelectedPlayer is not null
                    ? $" for {SelectedPlayer.ToDiscordString()}" : ""
                )}"
            ),
            new ComponentSeparatorProperties()
        );

        if (SelectedPlayer is not null) {
            container.AddComponents(
                new ComponentSectionProperties(
                    Button(SelectedPlayer.IsPaused ? "Resume" : "Pause", ButtonStyle.Secondary, () => {
                        Command<PlayerPauseToggle>().Execute();
                    })
                ).AddComponents(
                    new TextDisplayProperties(
                        $"## {DiscordString.Bold(DiscordString.Code(
                            SelectedPlayer.IsPaused ? "Paused" : "Resumed"
                        ))}"
                    )
                )
            );
        }
        else {
            container.AddComponents(
                new TextDisplayProperties("-# _No Selected Player_")
            );
        }

        return container;
    }
}
