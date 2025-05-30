﻿using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories.Dynamic
{
    public class PamelloPlayerRepository : IPamelloRepository<PamelloPlayer>, IDisposable
    {
        private readonly IServiceProvider _services;

        private AudioModel _globalAudio;

        private DiscordClientService _discordClients;
        private PamelloSpeakerRepository _speakers;

        private readonly List<PamelloPlayer> _players;
        public IReadOnlyList<PamelloPlayer> Players
            => _players;

        public PamelloPlayerRepository(IServiceProvider services

        ) {
            _services = services;
            
            _globalAudio = services.GetRequiredService<AudioModel>();
            
            _players = new List<PamelloPlayer>();
        }

        public void InitServices() {
            _discordClients = _services.GetRequiredService<DiscordClientService>();
            _speakers = _services.GetRequiredService<PamelloSpeakerRepository>();
        }

        public PamelloPlayer Create(PamelloUser creator, string name = "Player") {
            string oldName = name;
            for (int i = 1; _players.Any(player => player.Name == name); i++) {
                name = $"{oldName}-{i}";
            }

            var player = _globalAudio.AddModule(new PamelloPlayer(_globalAudio, _services, name, creator));
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

        public Task<IEnumerable<PamelloPlayer>> SearchAsync(string querry, PamelloUser? scopeUser) {
            var results = new List<PamelloPlayer>();
            if (scopeUser is null) return Task.FromResult((IEnumerable<PamelloPlayer>)results);

            querry = querry.ToLower();

            List<PamelloPlayer> vcPlayers = null;

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

            return Task.FromResult((IEnumerable<PamelloPlayer>)results);
        }

        public async Task<PamelloPlayer> GetByValueRequired(string value, PamelloUser? scopeUser)
            => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Cant find required player wuth id {value}");
        public async Task<PamelloPlayer?> GetByValue(string value, PamelloUser? scopeUser) {
            PamelloPlayer? player;

            if (value == "current") {
                player = scopeUser?.SelectedPlayer;
            }
            else if (value == "random") {
                var availablePlayers = (await SearchAsync("", scopeUser)).ToList();
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
                player.Dispose();
            }
        }
    }
}