using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Events.Creative;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Creative)]

[Safe<IPamelloSong>("Song")]
public partial class SongCreated : IRevertiblePamelloEvent, IPamelloEvent
{
    public partial class Pack
    {
        protected override void RevertInternal(IPamelloUser scopeUser) {
            var songs = Services.GetRequiredService<IPamelloSongRepository>();
            songs.Delete(Event._safeSong.RequiredEntity, scopeUser);
        }

        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return Event.Song is not null;
        }
    }
}

