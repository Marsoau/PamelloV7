using Discord;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class PlayerPauseToggleBuilder : PamelloComponentBuilder
{
    public ComponentBuilderV2 Component() {
        ContainerBuilder containerBuilder;
        var componentBuilder = new ComponentBuilderV2();

        componentBuilder = new ComponentBuilderV2()
            .WithContainer(containerBuilder = new ContainerBuilder()
                .WithTextDisplay($"### Pause Toggle{(SelectedPlayer is null ? ""
                    : $" for {SelectedPlayer.ToDiscordString()}"
                )}")
                .WithSeparator()
            );

        if (SelectedPlayer is not null) {
            containerBuilder
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId("player-pause-toggle")
                        .WithLabel(SelectedPlayer.IsPaused ? "Resume" : "Pause")
                        .WithStyle(ButtonStyle.Secondary)
                    )
                    .WithTextDisplay($"## {DiscordString.Bold(DiscordString.Code(
                        SelectedPlayer.IsPaused ? "Paused" : "Resumed"
                    ))}")
                );
        }
        else {
            containerBuilder
                .WithTextDisplay("-# _No Selected Player_");
        }
        
        return componentBuilder;
    }
}
