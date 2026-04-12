using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("help", "Display help")]
public partial class Help
{
    public async Task Execute() {
        await RespondAsync(() =>
            Builder<Builder>().Build()
        , () => []);
    }

    public class Builder : DiscordComponentBuilder
    {
        public enum HelpCategory
        {
            Introduction,
            Guides,
            Commands,
        }
        
        public HelpCategory Category { get; private set; } = HelpCategory.Introduction;
        
        public IMessageComponentProperties?[] Build() {
            var container = new ComponentContainerProperties();

            container.AddComponents(
                Select(Category, async category => {
                    Category = category;
                    await Message.Refresh();
                }),
                new TextDisplayProperties(Category.ToString())
            );

            return [
                container,
                Builder<BasicButtonsBuilder>().RefreshButtonRow()
            ];
        }
    }
}
