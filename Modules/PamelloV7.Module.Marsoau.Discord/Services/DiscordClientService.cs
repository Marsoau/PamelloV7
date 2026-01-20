using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Config;

namespace PamelloV7.Module.Marsoau.Discord.Services;

public class DiscordClientService : IPamelloService
{
	private readonly IServiceProvider _services;

	public DiscordSocketClient[] DiscordClients;

	public DiscordSocketClient Main {
		get => DiscordClients[0];
	}

	public DiscordClientService(IServiceProvider services) {
		_services = services;

		DiscordClients = new DiscordSocketClient[1];

		DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
	}

	public bool IsClientUser(ulong userId) {
		return DiscordClients.Any(client => client.CurrentUser.Id == userId);
	}

	public SocketVoiceChannel? GetUserVoiceChannel(IPamelloUser user) {
		SocketVoiceChannel? vc = null;
		foreach (var client in DiscordClients) {
			foreach (var guild in client.Guilds) {
				//vc = guild.GetUser(user.DiscordId)?.VoiceChannel;
				if (vc is not null) return vc;
			}
		}

		return null;
	}

	public SocketUser? GetDiscordUser(ulong userId) {
		foreach (var client in DiscordClients) {
			var user = client.GetUser(userId);
			if (user is not null) return user;
		}

		return null;
	}
	
	public void Shutdown() {
		foreach (var client in DiscordClients) {
			client.StopAsync().GetAwaiter().GetResult();
			client.LogoutAsync().GetAwaiter().GetResult();
		}
	}

	public void Dispose() {
		foreach (var client in DiscordClients) {
			client.Dispose();
		}
	}
}
