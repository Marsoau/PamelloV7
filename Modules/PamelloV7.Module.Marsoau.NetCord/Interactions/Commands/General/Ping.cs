using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
public partial class Ping
{
    public async Task Execute() {
        //await RespondAsync("Pong!", $"Hi {ScopeUser.ToDiscordString()}!");

        var button = Tokenizer.ModalButton("Modal", ButtonStyle.Secondary);
        
        await RespondAsync(() => [
            new ActionRowProperties().AddComponents(
                button
            )
        ]);
        
        return;
        await Interaction.SendResponseAsync(InteractionCallback.Modal(new ModalProperties(
            "modal-id",
            "Modal Title"
        ).AddComponents(
            new LabelProperties("One Line", new TextInputProperties("input-one", TextInputStyle.Short)),
            new LabelProperties("Many Line", new TextInputProperties("input-many", TextInputStyle.Paragraph)),
            new LabelProperties("Select", new StringMenuProperties("menu-id", [
                new StringMenuSelectOptionProperties("label-1", "value-1"),
                new StringMenuSelectOptionProperties("label-2", "value-2"),
                new StringMenuSelectOptionProperties("label-3", "value-3")
            ])),
            new LabelProperties("Check", new CheckboxProperties("check-id")),
            new LabelProperties("Radio", new CheckboxGroupProperties("radio-id", [
                new CheckboxGroupOptionProperties("ch-1", "value-1"),
                new CheckboxGroupOptionProperties("ch-2", "value-2").WithDefault(),
                new CheckboxGroupOptionProperties("ch-3", "value-3"),
            ]))
        )));
        
        return;
        await RespondAsync(() => [
            new ActionRowProperties().AddComponents(
                Button("Test", ButtonStyle.Danger, () => {
                    Output.Write("Test Button Click");
                    Interaction.SendFollowupMessageAsync(new InteractionMessageProperties() {
                        Content = "Test Button Click Message",
                        Flags = MessageFlags.Ephemeral
                    });
                })
            )
        ]);
    }
}
