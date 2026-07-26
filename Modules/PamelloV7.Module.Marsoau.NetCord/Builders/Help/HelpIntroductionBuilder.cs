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
                ## Welcome to PamelloV7!
                
                You can check out **Guides** by selecting them at the top of this message
                
                Also if you need any help, feel free to join the [discord server]()
                """
            )
        );
        
        return [
            container
        ];
    }
}
