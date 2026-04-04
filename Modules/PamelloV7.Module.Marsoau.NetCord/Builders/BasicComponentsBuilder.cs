using NetCord.Rest;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

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
    
    public IMessageComponentProperties?[] EntitiesList(string title, IEnumerable<IPamelloEntity> entitiesEnumerable, int page, bool displayEntityName = false, string noResultsMessage = "Nema rezultata") {
        var entities = entitiesEnumerable.ToList();
        const int pageSize = 10;

        var totalPages = entities.Count / pageSize + (entities.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;

        return [
            Info(title, entities.Count == 0
                ? noResultsMessage
                : string.Join("\n", entities.Skip(page * pageSize).Take(pageSize).Select(entity =>
                    $"{(displayEntityName ? $"{DiscordString.Code(entity.GetType().Name)} " : "")}{entity.ToDiscordString()}"
                ))
            ),
            Builder<BasicButtonsBuilder>().PageButtons(page != 0, page < totalPages - 1)
        ];
    }
    
    public TextDisplayProperties Loading() {
        return new TextDisplayProperties("-# _loading..._");
    }
}
