using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Services;
using Discord;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Handlers
{
	public class InteractionHandler {
		private readonly DiscordClientService _clients;
		private readonly InteractionService _commands;

		private readonly PamelloUserRepository _users;

		private readonly IServiceProvider _services;

		public InteractionHandler(
			DiscordClientService clients,
			InteractionService discordCommands,

			PamelloUserRepository users,

			IServiceProvider services
		) {
			_clients = clients;
			_commands = discordCommands;

			_users = users;

			_services = services;
		}

		public async Task InitializeAsync() {
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_clients.MainClient.InteractionCreated += InteractionCreated;
            _commands.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task InteractionCreated(SocketInteraction interaction) {
			try {
				await HandleInteraction(interaction);
			}
			catch (Exception x) {
				Console.WriteLine("ERROR with interaction");
                Console.WriteLine($"Message: {x.Message}");
                Console.WriteLine($"More: {x}");
				if (interaction.GetOriginalResponseAsync() is not null) {
					await interaction.FollowupAsync("An error occured, check the console for more info", ephemeral: true);
				}
				else {
                    await interaction.RespondAsync("An error occured, check the console for more info", ephemeral: true);
				}
			}
		}

		private async Task HandleInteraction(SocketInteraction interaction) {
			await interaction.DeferAsync(true);

			var pamelloUser = _users.GetByDiscord(interaction.User.Id);
			if (pamelloUser is null) {
				await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildException("Unexpected user error ocured"), true);
				throw new Exception("Unexpected user error ocured");
			}

			var pamelloContext = new PamelloSocketInteractionContext(_services, interaction, pamelloUser);

			await _commands.ExecuteCommandAsync(pamelloContext, _services);
        }

        private async Task SlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, Discord.Interactions.IResult result) {
			SocketInteraction interaction = context.Interaction as SocketInteraction ?? throw new Exception("tf");

			if (!result.IsSuccess && result is ExecuteResult executionResult) {
                if (executionResult.Exception?.InnerException is PamelloException pamelloException) {
                    await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(pamelloException.Message));
                }
				else if (executionResult.Exception?.InnerException is NotImplementedException) {
                    await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildException("Command is not implemented yet"));
				}
                else {
                    await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildException("Exception occured"));
                    Console.WriteLine($"|| EXCEPTION OCCURED IN COMMAND\n|| {commandInfo.Name}\n|| DESCRIPTION:");
                    Console.WriteLine(executionResult.Exception?.InnerException);
                    Console.WriteLine($"|| DESCRIPTION END\n||\n||");
                }
            }
		}
    }
}
