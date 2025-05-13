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

        [HttpGet("Search/Users")]
        [HttpGet("Search/Users/{querry}")]
        public async Task<IActionResult> SearchUsers(string querry = "")
            => await HandleBasicSearchRequest(querry, _users);

        [HttpGet("Search/Songs")]
        [HttpGet("Search/Songs/{querry}")]
        public async Task<IActionResult> SearchSongs(string querry = "", string? addedby = null, string? favoriteby = null) {
            PamelloUser? addedByUser = null;
            PamelloUser? favoriteByUser = null;

            if (addedby is not null) addedByUser = await _users.GetByValueRequired(addedby);
            if (favoriteby is not null) favoriteByUser = await _users.GetByValueRequired(favoriteby);

            if (addedByUser is null && favoriteByUser is null) {
                return await HandleBasicSearchRequest(querry, _songs);
            }

            var entityResults = await _songs.Search(querry, addedByUser, favoriteByUser, User);
            var idResults = entityResults.Select(entity => entity.Id);

            return Ok(idResults);
        }

        [HttpGet("Search/Episodes")]
        [HttpGet("Search/Episodes/{querry}")]
        public async Task<IActionResult> SearchEpisodes(string querry = "")
            => await HandleBasicSearchRequest(querry, _episodes);

        [HttpGet("Search/Playlists")]
        [HttpGet("Search/Playlists/{querry}")]
        public async Task<IActionResult> SearchPlaylists(string querry = "", string? addedby = null, string? favoriteby = null) {
            PamelloUser? addedByUser = null;
            PamelloUser? favoriteByUser = null;

            if (addedby is not null) addedByUser = await _users.GetByValueRequired(addedby);
            if (favoriteby is not null) favoriteByUser = await _users.GetByValueRequired(favoriteby);

            if (addedByUser is null && favoriteByUser is null) {
                return await HandleBasicSearchRequest(querry, _songs);
            }

            var entityResults = await _songs.Search(querry, addedByUser, favoriteByUser, User);
            var idResults = entityResults.Select(entity => entity.Id);

            return Ok(idResults);
        }

        [HttpGet("Search/Players")]
        [HttpGet("Search/Players/{querry}")]
        public async Task<IActionResult> SearchPlayers(string querry = "")
            => await HandleBasicSearchRequest(querry, _players);


        private async Task<IActionResult> HandleGetEntityRequest<T>(IPamelloRepository<T> repository)
            where T : class, IPamelloEntity
        {
			var qToken = Request.Query["token"].FirstOrDefault();

            if (qToken is null) RequireUser();

			var qId = Request.Query["id"].FirstOrDefault();
			var qValue = Request.Query["value"].FirstOrDefault();

            T entity;
            if (qToken is not null) {
                if (typeof(T) != typeof(PamelloUser))
                    throw new PamelloControllerException(BadRequest("only user can be requested by token"));
                if (!Guid.TryParse(qToken, out var token))
                    throw new PamelloControllerException(BadRequest("token must be a guid"));

                entity = await repository.GetByValueRequired(token.ToString());
            }
            else if (qId is not null) {
                if (!int.TryParse(qId, out var id))
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

            Console.WriteLine($"[Data Get {entity.GetType().Name}] {User?.ToString() ?? "Unknown User"}: {entity}");

			return Ok(entity.GetDTO());
        }

        private async Task<IActionResult> HandleBasicSearchRequest<T>(string querry, IPamelloRepository<T> repository)
            where T : IPamelloEntity
        {
            RequireUser();

            var entityResults = await repository.Search(querry, User);
            var idResults = entityResults.Select(entity => entity.Id);

            return Ok(idResults);
        }
    }
}
