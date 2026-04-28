using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Actions;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Base;

public abstract class DiscordComponentBuilder : DiscordBasicActions
{
    private Differentiator _differentiator = null!;
    
    public UpdatableMessage Message { get; private set; } = null!;
    
    public virtual void InitializeComponentBuilder(Differentiator differentiator, IServiceProvider services, IPamelloUser scopeUser) {
        InitializeActions(services, scopeUser);
        
        var messages = services.GetRequiredService<UpdatableMessageService>();
        Message = messages.Get(differentiator) ?? throw new PamelloException($"Message for component {GetType().Name} by {differentiator} differentiator not found");
        
        _differentiator = differentiator;
    }

    public override Differentiator GetCallSiteInteractionDifferentiator()
        => _differentiator;

    public static string GetEntriesText(IEnumerable<PamelloQueueEntry> list, int nextPosition, int countStart) {
        return GetEntriesText(list.Select(entry => entry.Song).OfType<IPamelloSong>(), countStart, nextPosition, DiscordString.Italic("next >"));
    }
    
    public static string GetEntriesText(IEnumerable<IPamelloEntity> items, int countStart, int changePosition = -1, string changePositionText = "") {
        var sb = new StringBuilder();

        foreach (var entry in items) {
            sb.AppendLine(
                $"{DiscordString.Code(countStart + 1)} {(countStart == changePosition ? changePositionText : ":")} {entry.ToDiscordString()}"
            );

            countStart++;
        }

        return sb.ToString();
    }
}
