using PamelloV7.Framework.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

[AutoInherit(typeof(DiscordModal))]
[RequiredMethodName("Submit")]

[AttributeUsage(AttributeTargets.Class)]
public class DiscordModalAttribute : Attribute
{
    public string Title { get; set; }
    
    public DiscordModalAttribute(string title) {
        Title = title;
    }
}
