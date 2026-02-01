using Discord.WebSocket;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Config;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Services
{
    public class DiscordClientService : IDisposable
    {
		private readonly IServiceProvider _services;

		private IPamelloSpeakerRepository _speakers;

		private IPamelloUserRepository _users;

		public DiscordSocketClient[] DiscordClients;

		public DiscordSocketClient MainClient {
			get => DiscordClients[0];
		}

		public DiscordClientService(IServiceProvider services) {
			_services = services;

			DiscordClients = new DiscordSocketClient[ServerConfig.Root.Discord.Tokens.SpeakerTokens.Length + 1];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			for (int i = 0; i < ServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
				DiscordClients[i + 1] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i + 1}");
			}
        }

		public void SubscriveToEvents() {
			if (_speakers is not null) return;

			_speakers = _services.GetRequiredService<IPamelloSpeakerRepository>();
			_users = _services.GetRequiredService<IPamelloUserRepository>();

			foreach (var client in DiscordClients) {
                client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
			}
		}

        private async Task Client_UserVoiceStateUpdated(SocketUser discordUser, SocketVoiceState fromVc, SocketVoiceState toVc) {
	        /*
	        if (_speakers is null) return;

	        List<IPamelloPlayerOld>? playersFromVc = null;
	        List<IPamelloPlayerOld>? playersToVc = null;

	        var user = _users.GetByDiscord(discordUser.Id);
	        if (user is null) return;

	        if (fromVc.VoiceChannel is not null) {
		        playersFromVc = _speakers.GetVoicePlayers(fromVc.VoiceChannel.Id);
	        }
	        if (toVc.VoiceChannel is not null) {
		        playersToVc = _speakers.GetVoicePlayers(toVc.VoiceChannel.Id);
	        }

	        playersFromVc ??= new List<IPamelloPlayerOld>();
	        playersToVc ??= new List<IPamelloPlayerOld>();

	        if (playersToVc.Count == 1) {
		        user.SelectedPlayer = playersToVc.First();
	        }
	        else if (playersToVc.Count > 1 && user.PreviousPlayer is not null && playersToVc.Contains(user.PreviousPlayer)) {
		        user.SelectedPlayer = user.PreviousPlayer;
	        }
	        else if (user.SelectedPlayer is not null && playersFromVc.Contains(user.SelectedPlayer)) {
                user.SelectedPlayer = null;
            }

            Console.WriteLine($"VCC Auto selected player {user.SelectedPlayer?.ToString() ?? "NULL"} for {user}");
	        */

	        /*
	        if (_speakers is null) return;
	        if (toVc.VoiceChannel is null) return;

	        var pamelloUser = _users.GetByDiscord(user.Id);
	        if (pamelloUser is null) return;

	        PamelloPlayer? player = null;

            var vcPlayers = _speakers.GetVoicePlayers(toVc.VoiceChannel.Id);
            if (vcPlayers.Count == 1) player = vcPlayers.First();

            Console.WriteLine($"VCC Auto selected player {player?.ToString() ?? "NULL"} for {pamelloUser}");
	        pamelloUser.SelectedPlayer = player;
	        */
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

        public List<IPamelloUser> GetVoiceChannelUsers(SocketVoiceChannel vc) {
			var users = new List<IPamelloUser>();

			IPamelloUser? pamelloUser = null;
			foreach (var discordUser in vc.ConnectedUsers) {
				pamelloUser = _users.GetByDiscord(discordUser.Id, false);
				if (pamelloUser is null) continue;

				users.Add(pamelloUser);
			}

			return users;
		}

		public SocketUser? GetDiscordUser(ulong userId) {
			SocketUser? user = null;

			foreach (var client in DiscordClients) {
				user = client.GetUser(userId);
				if (user is not null) break;
			}

			return user;
		}

		public void Dispose() {
			Console.WriteLine("Disposing discord clients");
			
			foreach (var client in DiscordClients) {
				client.LogoutAsync().Wait();
				client.Dispose();
			}
		}
    }
}
