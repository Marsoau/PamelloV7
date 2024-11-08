using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using Discord;

namespace PamelloV7.Server.Handlers
{
	public class InteractionHandler {
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _commands;

		private readonly IServiceProvider _services;

		public InteractionHandler(
			DiscordSocketClient client,
			InteractionService discordCommands,

			IServiceProvider services
		) {
			_client = client;
			_commands = discordCommands;

			_services = services;
		}

		public async Task InitializeAsync() {
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_client.InteractionCreated += InteractionCreated;
        }

        private async Task InteractionCreated(SocketInteraction interaction) {
			try {
				await HandleInteraction(interaction);
			}
			catch (Exception x) {
				Console.WriteLine("ERROR with interaction");
                Console.WriteLine($"Message: {x.Message}");
                Console.WriteLine($"More: {x}");
                await interaction.RespondAsync("An error occured, check the console for more info", ephemeral: true);
			}
		}

		private async Task HandleInteraction(SocketInteraction interaction) {
			var context = new SocketInteractionContext(_client, interaction);
			await _commands.ExecuteCommandAsync(context, _services);
        }
    }
}
