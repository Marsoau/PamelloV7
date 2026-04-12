using Microsoft.Extensions.DependencyInjection;
using NetCord;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Extensions;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

public class TokenizedSelectInteraction<TType> : TokenizedInteraction
{
    private readonly IServiceProvider _services;
    
    public Func<TType, Task> ExecuteOnSelect { get; }
    
    public TokenizedSelectInteraction(
        InteractionCallSite callSite,
        Func<TType, Task> action,
        IServiceProvider services
    ) : base(callSite) {
        _services = services;
        
        ExecuteOnSelect = action;
        
        Action = async interaction => {
            if (interaction is not StringMenuInteraction buttonInteraction) return;
            
            await ExecuteSelectAsync(buttonInteraction);
        };
    }

    public async Task ExecuteSelectAsync(StringMenuInteraction interaction) {
        var users = _services.GetRequiredService<IPamelloUserRepository>();
        var scopeUser = await users.GetByInteractionRequired(interaction);
        
        var value = interaction.Data.SelectedValues.FirstOrDefault();
        if (value is null) return;
        
        var converted = await PamelloStaticActions.ConvertStringAsync<TType>(
            value,
            "",
            _services.GetRequiredService<IEntityQueryService>(),
            scopeUser
        );
        
        await ExecuteOnSelect(converted);
    }
}
