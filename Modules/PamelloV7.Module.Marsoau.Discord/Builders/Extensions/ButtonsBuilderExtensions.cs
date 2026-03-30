using Discord;

namespace PamelloV7.Module.Marsoau.Discord.Builders.Extensions;

public static class ButtonsBuilderExtensions
{
    extension(ComponentBuilderV2 builder)
    {
        public ComponentBuilderV2 WithRefresh() {
            return new ButtonsBuilder().RefreshButton(builder);
        }
    }
}
