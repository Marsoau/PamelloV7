using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesClear
{
    public IHistoryRecord? Execute() {
        return IEventsService.FirstRecordIn(
            () => ScopeUser.ClearFavoriteSongs()
        );
    }
}
