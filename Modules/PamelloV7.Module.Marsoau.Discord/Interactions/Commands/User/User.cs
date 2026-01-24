using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.User;

[Map]
[Group("user", "Actions with users")]
public partial class User : DiscordCommand
{ }

[Map]
public partial class UserInteractions : DiscordCommand
{ }
