using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class BasicComponentsBuilder : DiscordComponentBuilder
{
    public ComponentContainerProperties Info(string? title, string? content) {
        return new ComponentContainerProperties().AddComponents(
            new TextDisplayProperties(
                (title?.Length > 0 ? $"### {title}\n" : "") +
                (content?.Length > 0 ? $"{content}\n" : "")
            )
        );
    }
    
    public ComponentContainerProperties Loading() {
        return new ComponentContainerProperties().AddComponents(
            new TextDisplayProperties("-# _loading..._")
        );
    }
}
