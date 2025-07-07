using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Extensions;

public static class DiscordStringExtensions
{
    public static DiscordString ToDiscordString(this IPamelloEntity entity) {
        return DiscordString.Bold(entity.Name) + " " + DiscordString.Code($"[{entity.Id}]");
    }
    
    public static string ToDiscordFooterString(this IPamelloPlayer player) {
        return $"{player.Name} [{player.Id}] {(player.IsProtected ? " (private)" : "")}";
    }
}
