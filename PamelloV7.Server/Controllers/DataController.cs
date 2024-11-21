using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Model;
using PamelloV7.Server.Repositories;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;

        public DataController(IServiceProvider services) {
            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
        }

		[HttpGet("Song")]
		public IActionResult GetSong() {
			return HandleGetByIdRequest(_songs);
        }

        private IActionResult HandleGetByIdRequest<T>(IPamelloRepository<T> repository)
            where T : IEntity
        {
			var qId = Request.Query["id"].FirstOrDefault();
			if (qId is null) {
				return BadRequest("Id required");
			}

			if (!int.TryParse(qId, out int id)) {
				return BadRequest("Id must be an integer number");
			}

			var pamelloEntity = repository.Get(id);
			if (pamelloEntity is null) return NotFound();

			return Ok(pamelloEntity.DTO);
        }
    }
}
