using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Model.Audio;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlayerRepository
    {
        private readonly IServiceProvider _services;

        private readonly List<PamelloPlayer> _players;

        public PamelloPlayerRepository(IServiceProvider services

        ) {
            _services = services;
            
            _players = new List<PamelloPlayer>();
        }

        public PamelloPlayer Create(string name = "Player") {
			string oldName = name;
			for (int i = 1; _players.Any(player => player.Name == name); i++) {
				name = $"{oldName}-{i}";
			}

            var player = new PamelloPlayer(_services, name);
            _players.Add(player);

            return player;
        }

        public PamelloPlayer GetRequired(int id)
            => Get(id) ?? throw new PamelloException($"Cant find required played wuth id {id}");
        public PamelloPlayer? Get(int id) {
            return _players.FirstOrDefault(player => player.Id == id);
        }

        public PamelloPlayer? GetByName(string name) {
            return _players.FirstOrDefault(player => player.Name == name);
        }

        public PamelloPlayer? GetByValue(string value) {
            PamelloPlayer? player = null;

            if (int.TryParse(value, out int id)) {
                player = Get(id);
            }

            player ??= GetByName(value);

            return player;
        }
    }
}
