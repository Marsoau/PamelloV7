using Discord.Interactions;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Context;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Base;

public interface IPamelloInteractionActions
{
    public IPamelloUser User { get; }
    public IServiceProvider Services { get; }
    
    public IEntityQueryService PEQL { get; }
    
    public Task RespondLoading();
    
}
