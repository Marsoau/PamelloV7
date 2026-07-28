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
                
                Feel free to join the [Discord server](https://discord.gg/JCgXM2ARFV) if you need any additional help, or to give your feedback / suggestion / report a problem
                """
            )
        );
        
        return [
            container
        ];
    }
}
