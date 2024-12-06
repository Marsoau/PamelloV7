using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Exceptions;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : PamelloControllerBase
    {
        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;
        private readonly PamelloPlayerRepository _players;

        public DataController(IServiceProvider services) : base(services) {
            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            _players = services.GetRequiredService<PamelloPlayerRepository>();
        }

		[HttpGet("User")]
		public async Task<IActionResult> GetUser()
			=> await HandleGetEntityRequest(_users);

		[HttpGet("Song")]
		public async Task<IActionResult> GetSong()
			=> await HandleGetEntityRequest(_songs);

		[HttpGet("Episode")]
		public async Task<IActionResult> GetEpisode()
			=> await HandleGetEntityRequest(_episodes);

		[HttpGet("Playlist")]
		public async Task<IActionResult> GetPlaylist()
			=> await HandleGetEntityRequest(_playlists);

		[HttpGet("Player")]
		public async Task<IActionResult> GetPlayer()
			=> await HandleGetEntityRequest(_players);


        private async Task<IActionResult> HandleGetEntityRequest<T>(IPamelloRepository<T> repository)
            where T : IEntity
        {
            RequireUser();

			var qId = Request.Query["id"].FirstOrDefault();
			var qValue = Request.Query["value"].FirstOrDefault();

            IEntity entity;
            if (qId is not null) {
                if (!int.TryParse(qId, out int id))
                    throw new PamelloControllerException(BadRequest("id must be an integer number"));

                entity = repository.Get(id) ??
                    throw new PamelloControllerException(NotFound($"entity with id {id} not found"));
            }
            else if (qValue is not null) {
                entity = await repository.GetByValue(qValue, User) ??
                    throw new PamelloControllerException(NotFound($"entity with value \"{qValue}\" not found"));
            }
            else {
                throw new PamelloControllerException(BadRequest("id or value required"));
            }

            Console.WriteLine($"[Data Get {entity.GetType().Name}] {User}: {entity}");

			return Ok(entity.GetDTO());
        }
    }
}
