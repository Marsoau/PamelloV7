using Discord.Interactions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Context;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Base;

public interface IPamelloInteractionActions
{
    public IPamelloUser User { get; }
    public IServiceProvider Services { get; }
    
    public IEntityQueryService PEQL { get; }
    
    public Task RespondLoading();
    
}
