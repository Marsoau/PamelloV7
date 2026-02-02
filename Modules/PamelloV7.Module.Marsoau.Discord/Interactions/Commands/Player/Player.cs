using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

[Map]
[Group("player", "Actions with players")]
public partial class Player : DiscordCommand
{
    [Group("queue", "Actions with favorite songs")]
    public class PlayerQueueMap : PlayerQueue
    { }
}

[Map]
public partial class PlayerInteractions : DiscordCommand
{ }
