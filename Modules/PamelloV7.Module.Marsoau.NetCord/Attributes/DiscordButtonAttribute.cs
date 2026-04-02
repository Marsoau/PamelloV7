using NetCord;

namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

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
