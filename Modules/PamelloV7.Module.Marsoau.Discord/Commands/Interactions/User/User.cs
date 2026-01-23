using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.User;

[Map]
[Group("user", "Actions with users")]
public partial class User : DiscordCommand
{ }

[Map]
public partial class UserInteractions : DiscordCommand
{ }
