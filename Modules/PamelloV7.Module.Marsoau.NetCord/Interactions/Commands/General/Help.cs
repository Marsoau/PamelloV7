using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Help;

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
            var categoryComponents = Category switch {
                HelpCategory.Introduction => Builder<HelpIntroductionBuilder>().Build(),
                HelpCategory.Guides => Builder<HelpGuidesBuilder>().Build(),
                _ => []
            };
            
            return [
                Select(Category, async category => {
                    Category = category;
                    await Message.Refresh();
                }),
                ..categoryComponents,
                Builder<BasicButtonsBuilder>().RefreshButtonRow()
            ];
        }
    }
}
