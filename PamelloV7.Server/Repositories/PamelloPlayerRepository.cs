using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlayerRepository : IPamelloRepository<PamelloPlayer>
    {
        private readonly IServiceProvider _services;

        private DiscordClientService _discordClients;
        private PamelloSpeakerService _speakers;

        private readonly List<PamelloPlayer> _players;

        public PamelloPlayerRepository(IServiceProvider services

        ) {
            _services = services;
            
            _players = new List<PamelloPlayer>();
        }

        public void InitServices() {
            _discordClients = _services.GetRequiredService<DiscordClientService>();
            _speakers = _services.GetRequiredService<PamelloSpeakerService>();
        }

        public PamelloPlayer Create(PamelloUser creator, string name = "Player") {
			string oldName = name;
			for (int i = 1; _players.Any(player => player.Name == name); i++) {
				name = $"{oldName}-{i}";
			}

            var player = new PamelloPlayer(_services, name, creator);
            _players.Add(player);

            return player;
        }

        public PamelloPlayer GetRequired(int id)
            => Get(id) ?? throw new PamelloException($"Cant find required player wuth id {id}");
        public PamelloPlayer? Get(int id) {
            return _players.FirstOrDefault(player => player.Id == id);
        }

        public PamelloPlayer? GetByName(string name) {
            return _players.FirstOrDefault(player => player.Name == name);
        }

        public List<PamelloPlayer> Search(string querry, PamelloUser scopeUser) {
            var results = new List<PamelloPlayer>();
            querry = querry.ToLower();

            List<PamelloPlayer> vcPlayers = null;

            var vc = _discordClients.GetUserVoiceChannel(scopeUser);
            if (vc is not null) vcPlayers = _speakers.GetVoicePlayers(vc.Id);
            else vcPlayers = new List<PamelloPlayer>();

            foreach (var pamelloPlayer in _players) {
                if (pamelloPlayer is null) continue;

                if (pamelloPlayer.Name.ToLower().Contains(querry)) {
                    if (pamelloPlayer.IsProtected) {
                        if (pamelloPlayer.Creator != scopeUser) {
                            if (!vcPlayers.Contains(pamelloPlayer)) {
                                continue;
                            }
                        }
                    }
                    results.Add(pamelloPlayer);
                }
            }

            return results;
        }

        public async Task<PamelloPlayer> GetByValueRequired(string value, PamelloUser? scopeUser = null)
            => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Cant find required player wuth id {value}");
        public Task<PamelloPlayer?> GetByValue(string value, PamelloUser? scopeUser = null) {
            PamelloPlayer? player = null;

            if (value == "current") {
                player = scopeUser?.SelectedPlayer;
            }
            else if (int.TryParse(value, out int id)) {
                player = Get(id);
            }
            else {
                player = GetByName(value);
            }

            return Task.FromResult(player);
        }
    }
}
