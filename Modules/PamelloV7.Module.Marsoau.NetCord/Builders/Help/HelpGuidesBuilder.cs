using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help;

public class HelpGuidesBuilder : DiscordComponentBuilder
{
    public enum GuideCategory
    {
        AddingSongs,
        ManagingPlayer,
        ManagingQueue,
        ManagingSongs,
        ManagingSongEpisodes,
        ManagingPlaylists,
        ManagingFavorites,
        ManagingUser,
    }
    
    public GuideCategory Category { get; private set; } = GuideCategory.AddingSongs;
    
    public IMessageComponentProperties?[] Build() {
        var categoryComponents = Category switch {
            GuideCategory.AddingSongs => Builder<HelpGuideAddingSongsBuilder>().Build(),
            GuideCategory.ManagingPlayer => Builder<HelpGuideManagingPlayerBuilder>().Build(),
            GuideCategory.ManagingQueue => Builder<HelpGuideManagingQueueBuilder>().Build(),
            GuideCategory.ManagingSongs => Builder<HelpGuideManagingSongsBuilder>().Build(),
            GuideCategory.ManagingSongEpisodes => Builder<HelpGuideManagingSongEpisodesBuilder>().Build(),
            GuideCategory.ManagingPlaylists => Builder<HelpGuideManagingPlaylistsBuilder>().Build(),
            GuideCategory.ManagingFavorites => Builder<HelpGuideManagingFavoritesBuilder>().Build(),
            GuideCategory.ManagingUser => Builder<HelpGuideManagingUserBuilder>().Build(),
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
