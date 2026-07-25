using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help;

public class HelpGuidesBuilder : DiscordComponentBuilder
{
    public enum GuideCategory
    {
        AddingSongs,
    }
    
    public GuideCategory Category { get; private set; } = GuideCategory.AddingSongs;
    
    public IMessageComponentProperties?[] Build() {
        var categoryComponents = Category switch {
            GuideCategory.AddingSongs => Builder<HelpGuideAddingSongsBuilder>().Build(),
            _ => []
        };

        return [
            ..categoryComponents,
            Select(Category, async category => {
                Category = category;
                await Message.Refresh();
            })
        ];
    }
}
