using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help;

public class HelpIntroductionBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            new TextDisplayProperties(
                """
                ## The help menu is in development!
                """
            )
        );
        
        return [
            container
        ];
    }
}
