using PamelloV7.Framework.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

[AutoInherit(typeof(DiscordModalBuilder))]
[RequiredMethodName("Build")]

[AttributeUsage(AttributeTargets.Class)]
public class DiscordModalBuilderAttribute : Attribute
{
    
}
