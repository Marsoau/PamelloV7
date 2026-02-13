using Discord;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class ButtonsBuilder : PamelloComponentBuilder
{
    public ComponentBuilderV2 PageButtons(ComponentBuilderV2 pageBuilder, bool displayPrev, bool displayNext) {
        var actionRow = new ActionRowBuilder();

        if (displayPrev) {
            actionRow.WithButton(new ButtonBuilder()
                .WithCustomId("page-prev")
                .WithLabel("Prev")
                .WithStyle(ButtonStyle.Secondary)
            );
        }
        if (displayNext) {
            actionRow.WithButton(new ButtonBuilder()
                .WithCustomId("page-next")
                .WithLabel("Next")
                .WithStyle(ButtonStyle.Secondary)
            );
        }

        if (!displayPrev && !displayNext) return pageBuilder;
        
        pageBuilder.WithActionRow(actionRow);

        return pageBuilder;
    }
    
    public ComponentBuilderV2 RefreshButton(ComponentBuilderV2 componentBuilder) {
        componentBuilder
            .WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId("refresh")
                    .WithLabel("Refresh")
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        
        return componentBuilder;
    }
}
