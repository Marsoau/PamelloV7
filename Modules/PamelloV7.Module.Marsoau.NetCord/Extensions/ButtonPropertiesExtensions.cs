using NetCord.Rest;

namespace PamelloV7.Module.Marsoau.NetCord.Extensions;

public static class ButtonPropertiesExtensions
{
    public static ActionRowProperties InActionRow(this ButtonProperties properties) {
        return new ActionRowProperties().AddComponents(properties);
    }
}
