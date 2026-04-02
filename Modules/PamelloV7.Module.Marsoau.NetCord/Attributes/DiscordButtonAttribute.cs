using NetCord;
using PamelloV7.Framework.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

[AutoInherit(typeof(DiscordButton))]
[RequiredMethodName("Execute")]

[AttributeUsage(AttributeTargets.Class)]
public class DiscordButtonAttribute : Attribute
{
    public string Label { get; set; }
    public ButtonStyle Style { get; set; }
    
    public DiscordButtonAttribute(string label, ButtonStyle style = ButtonStyle.Secondary) {
        Label = label;
        Style = style;
    }
}
