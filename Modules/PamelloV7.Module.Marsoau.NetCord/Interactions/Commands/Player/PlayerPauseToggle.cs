using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player pause-toggle", "Toggle player pause state")]
public partial class PlayerPauseToggle
{
    public async Task Execute() {
        Command<Framework.Commands.PlayerPauseToggle>().Execute();
        
        await RespondAsync(() =>
            Builder<Builder>().Build()
        , () => [ScopeUser, SelectedPlayer]);
    }
    
    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties Build() {
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
                            Command<Framework.Commands.PlayerPauseToggle>().Execute();
                        })
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"## {DiscordString.Code(
                                SelectedPlayer.IsPaused ? "Paused" : "Resumed"
                            )}"
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
}
