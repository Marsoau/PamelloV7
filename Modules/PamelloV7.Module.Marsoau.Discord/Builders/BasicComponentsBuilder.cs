using Discord;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class BasicComponentsBuilder : PamelloDiscordComponentBuilder
{
    public ComponentBuilderV2 Info(string? content)
        => Info(null, content);
    public ComponentBuilderV2 Info(string? title, string? content) {
        return new ComponentBuilderV2()
            .WithContainer(new ContainerBuilder()
                .WithTextDisplay(
                    (title?.Length > 0 ? $"### {title}\n" : "") +
                    (content?.Length > 0 ? $"{content}\n" : "")
                )
            );
    }
    
    public ComponentBuilderV2 EntitiesList(string title, IEnumerable<IPamelloEntity> iEntities, int page, bool displayEntityName = false, string noResultsMessage = "Nema rezultata") {
        var entities = iEntities.ToList();
        
        const int pageSize = 10;

        var totalPages = entities.Count / pageSize + (entities.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;

        return Builder<ButtonsBuilder>().PageButtons(Info(title,
        entities.Count == 0 ? noResultsMessage
            : string.Join("\n", entities.Skip(page * pageSize).Take(pageSize).Select(entity =>
                    $"{(displayEntityName ? $"{DiscordString.Code(entity.GetType().Name)} " : "")}{entity.ToDiscordString()}"
            ))
        ), page != 0, page < totalPages - 1);
    }
    
    public ComponentBuilderV2 Defer() {
        return new ComponentBuilderV2().WithTextDisplay("-# _loading..._");
    }
}
