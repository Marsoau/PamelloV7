using Discord;
using Discord.WebSocket;

namespace PamelloV7.Server.Extensions
{
    public static class SocketInteractionExtensions
    {
        public static async Task RespondWithEmbedAsync(this SocketInteraction interaction, Embed embed, bool ephemeral = false) {
            if (interaction.GetOriginalResponseAsync() is not null) {
                await interaction.ModifyOriginalResponseAsync(message => message.Embed = embed);
            }
            else {
                await interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
            }
        }
        public static async Task RespondWithTextAsync(this SocketInteraction interaction, string messageContent, bool ephemeral = false) {
            if (interaction.GetOriginalResponseAsync() is not null) {
                await interaction.ModifyOriginalResponseAsync(message => message.Content = messageContent);
            }
            else {
                await interaction.RespondAsync(messageContent, ephemeral: ephemeral);
            }
        }
    }
}
