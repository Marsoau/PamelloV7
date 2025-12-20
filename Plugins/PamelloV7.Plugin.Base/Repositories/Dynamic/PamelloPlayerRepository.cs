/*
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories.Dynamic
{
    public class PamelloPlayerRepository : IPamelloPlayerRepository, IDisposable
    {
        private readonly IServiceProvider _services;

        private AudioModel _globalAudio;

        private DiscordClientService _discordClients;
        private IPamelloSpeakerRepository _speakers;

        private readonly List<IPamelloPlayer> _players;
        public IReadOnlyList<IPamelloPlayer> Players
            => _players;

        public PamelloPlayerRepository(IServiceProvider services

        ) {
            _services = services;
            
            _globalAudio = services.GetRequiredService<AudioModel>();
            
            _players = new List<IPamelloPlayer>();
        }

        //
        //TO DELETE
        public event Action? BeforeLoading;
        public event Action<int, int>? OnLoadingProgress;
        public event Action? OnLoaded;
        public event Action? BeforeInit;
        public event Action<int, int>? OnInitProgress;
        public event Action? OnInit;
        //TO DELETE
        //

        public void InitServices() {
            _discordClients = _services.GetRequiredService<DiscordClientService>();
            _speakers = _services.GetRequiredService<IPamelloSpeakerRepository>();
        }

        public Task LoadAllAsync() {
            throw new NotImplementedException();
        }

        public Task InitAllAsync() {
            throw new NotImplementedException();
        }

        public IPamelloPlayer Create(IPamelloUser creator, string name = "Player") {
            string oldName = name;
            for (int i = 1; _players.Any(player => player.Name == name); i++) {
                name = $"{oldName}-{i}";
            }

            var player = _globalAudio.AddModule(new PamelloPlayer(_globalAudio, _services, name, creator));
            _players.Add(player);

            return player;
        }

        public IEnumerable<IPamelloPlayer>? GetLoaded() {
            return Players;
        }

        public IPamelloPlayer GetRequired(int id)
            => Get(id) ?? throw new PamelloException($"Cant find required player wuth id {id}");
        public IPamelloPlayer? Get(int id) {
            return _players.FirstOrDefault(player => player.Id == id);
        }

        public IPamelloPlayer? GetByName(string name) {
            return _players.FirstOrDefault(player => player.Name == name);
        }

        public Task<IEnumerable<IPamelloPlayer>> SearchAsync(string querry, IPamelloUser? scopeUser)
            => Task.Run(() => Search(querry, scopeUser));

        public void Delete(IPamelloPlayer entity) {
            throw new NotImplementedException();
        }

        public IEnumerable<IPamelloPlayer> Search(string querry, IPamelloUser? scopeUser) {
            var results = new List<IPamelloPlayer>();
            if (scopeUser is null) return results;

            querry = querry.ToLower();

            List<IPamelloPlayer> vcPlayers = null;

            var vc = _discordClients.GetUserVoiceChannel(scopeUser);
            vcPlayers = vc is not null ? _speakers.GetVoicePlayers(vc.Id) : [];

            foreach (var pamelloPlayer in _players) {
                if (pamelloPlayer is null) continue;
                if (!pamelloPlayer.Name.Contains(querry, StringComparison.CurrentCultureIgnoreCase)) continue;
                
                if (pamelloPlayer.IsProtected) {
                    if (pamelloPlayer.Creator != scopeUser) {
                        if (!vcPlayers.Contains(pamelloPlayer)) {
                            continue;
                        }
                    }
                }
                
                results.Add(pamelloPlayer);
            }

            return results;
        }

        public async Task<IPamelloPlayer> GetByValueRequired(string value, IPamelloUser? scopeUser)
            => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Cant find required player wuth id {value}");
        public Task<IPamelloPlayer?> GetByValue(string value, IPamelloUser? scopeUser)
            => Task.Run(() => GetByValueSync(value, scopeUser));
        public IPamelloPlayer? GetByValueSync(string value, IPamelloUser? scopeUser) {
            IPamelloPlayer? player;

            if (value == "current") {
                player = scopeUser?.SelectedPlayer;
            }
            else if (value == "random") {
                var availablePlayers = (SearchAsync("", scopeUser).Result).ToList();
                var i = Random.Shared.Next(0, availablePlayers.Count);

                player = availablePlayers[i];
            }
            else if (int.TryParse(value, out var id)) {
                player = Get(id);
            }
            else {
                player = GetByName(value);
            }

            if (player is null) return null;
            if (player.Creator == scopeUser) return player;
            
            var vc = _discordClients.GetUserVoiceChannel(scopeUser);
            var vcPlayers = vc is not null ? _speakers.GetVoicePlayers(vc.Id) : [];
            
            return vcPlayers.Contains(player) ? player : null;
        }

        public void Dispose() {
            Console.WriteLine("Disposing players");
            
            foreach (var player in _players) {
                ((PamelloPlayer)player).Dispose();
            }
        }
    }
}

*/