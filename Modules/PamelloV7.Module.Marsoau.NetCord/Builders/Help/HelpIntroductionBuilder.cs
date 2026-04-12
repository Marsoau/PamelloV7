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
                # Welcome to PamelloV7!
                
                ### **PamelloV7** is a feature rich audio server, which acts and feels like a **music bot!**
                
                You can start from selecting a **`Guides`** category in the top of this message, and adding a song for the first time!
                """
            )
        );
        
        return [
            container
        ];
    }
}
