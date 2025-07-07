using Microsoft.AspNetCore.Mvc;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : PamelloControllerBase
    {
        private readonly IPamelloUserRepository _users;
        private readonly IPamelloSongRepository _songs;
        private readonly IPamelloEpisodeRepository _episodes;
        private readonly IPamelloPlaylistRepository _playlists;
        
        private readonly IPamelloPlayerRepository _players;
        private readonly IPamelloSpeakerRepository _speakers;

        public DataController(IServiceProvider services) : base(services) {
            _users = services.GetRequiredService<IPamelloUserRepository>();
            _songs = services.GetRequiredService<IPamelloSongRepository>();
            _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
            
            _players = services.GetRequiredService<IPamelloPlayerRepository>();
            _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();
        }

        [HttpGet("User/{value}")]
        public async Task<IActionResult> GetUser(string value)
            => await HandleGetEntityRequest(_users, value);

        [HttpGet("Song/{value}")]
        public async Task<IActionResult> GetSong(string value)
            => await HandleGetEntityRequest(_songs, value);

        [HttpGet("Episode/{value}")]
        public async Task<IActionResult> GetEpisode(string value)
            => await HandleGetEntityRequest(_episodes, value);

        [HttpGet("Playlist/{value}")]
        public async Task<IActionResult> GetPlaylist(string value)
            => await HandleGetEntityRequest(_playlists, value);

        [HttpGet("Player/{value}")]
        public async Task<IActionResult> GetPlayer(string value)
            => await HandleGetEntityRequest(_players, value);
        
        /*
        [HttpGet("Speaker/{value}")]
        public async Task<IActionResult> GetSpeaker(string value)
            => await HandleGetEntityRequest(_speakers, value);
        */

        [HttpGet("Search/Users")]
        [HttpGet("Search/Users/{querry}")]
        public async Task<IActionResult> SearchUsers(string querry = "")
            => await HandleBasicSearchRequest(querry, _users);

        [HttpGet("Search/Songs")]
        [HttpGet("Search/Songs/{querry}")]
        public async Task<IActionResult> SearchSongs(string querry = "", string? addedby = null, string? favoriteby = null) {
            RequireUser();
            
            IPamelloUser? addedByUser = null;
            IPamelloUser? favoriteByUser = null;

            if (addedby is not null) addedByUser = await _users.GetByValueRequired(addedby, User);
            if (favoriteby is not null) favoriteByUser = await _users.GetByValueRequired(favoriteby, User);

            if (addedByUser is null && favoriteByUser is null) {
                return await HandleBasicSearchRequest(querry, _songs);
            }

            var entityResults = _songs.Search(querry, User, addedByUser, favoriteByUser);
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
            RequireUser();
            
            IPamelloUser? addedByUser = null;
            IPamelloUser? favoriteByUser = null;

            if (addedby is not null) addedByUser = await _users.GetByValueRequired(addedby, User);
            if (favoriteby is not null) favoriteByUser = await _users.GetByValueRequired(favoriteby, User);

            if (addedByUser is null && favoriteByUser is null) {
                return await HandleBasicSearchRequest(querry, _songs);
            }

            var entityResults = _songs.Search(querry, User, favoriteByUser, favoriteByUser);
            var idResults = entityResults.Select(entity => entity.Id);

            return Ok(idResults);
        }

        [HttpGet("Search/Players")]
        [HttpGet("Search/Players/{querry}")]
        public async Task<IActionResult> SearchPlayers(string querry = "")
            => await HandleBasicSearchRequest(querry, _players);
        
        /*
        [HttpGet("Search/Speakers")]
        [HttpGet("Search/Speakers/{querry}")]
        public async Task<IActionResult> SearchSpeakers(string querry = "")
            => await HandleBasicSearchRequest(querry, _speakers);
        */


        private async Task<IActionResult> HandleGetEntityRequest<T>(IPamelloRepository<T> repository, string value)
            where T : class, IPamelloEntity
        {
            RequireUser();

            var entity = await repository.GetByValue(value, User) ??
                         throw new PamelloControllerException(NotFound($"entity with value \"{value}\" not found"));
            //throw new PamelloControllerException(BadRequest("id or value required"));

            Console.WriteLine($"[Data Get {entity.GetType().Name}] {User?.ToString() ?? "Unknown User"}: {entity}");

            return Ok(entity.GetDTO());
        }

        private async Task<IActionResult> HandleBasicSearchRequest<T>(string querry, IPamelloRepository<T> repository)
            where T : IPamelloEntity
        {
            RequireUser();

            var entityResults = await repository.SearchAsync(querry, User);
            var idResults = entityResults.Select(entity => entity.Id);

            return Ok(idResults);
        }
    }
}
