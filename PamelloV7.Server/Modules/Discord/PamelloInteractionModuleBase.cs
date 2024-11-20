using Discord;
using Discord.Interactions;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Audio;

namespace PamelloV7.Server.Modules.Discord
{
    public class PamelloInteractionModuleBase : InteractionModuleBase<PamelloSocketInteractionContext>
    {
        private readonly UserAuthorizationService _authorization;
        private readonly DiscordClientService _discordClients;

        private readonly PamelloPlayerRepository _players;
        private readonly PamelloSpeakerService _speakers;

        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;

        private PamelloCommandsModule Commands {
            get => Context.User.Commands;
        }
        private PamelloPlayer Player {
            get => Context.User.RequiredSelectedPlayer;
        }

        public PamelloInteractionModuleBase(IServiceProvider services)
        {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<PamelloPlayerRepository>();
            _speakers = services.GetRequiredService<PamelloSpeakerService>();

            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
        }

        protected async Task Respond(Embed embed) {
            await ModifyOriginalResponseAsync(message => message.Embed = embed);
        }
        protected async Task RespondInfo(string message, bool bold = false) {
            if (bold) await RespondInfo(message, "");
            else await RespondInfo("", message);
        }
        protected async Task RespondPage(string header, string content, int? page, int? totalPages) {
            await Respond(
                PamelloEmbedBuilder.Info(header, content)
                .WithFooter($"{
					(page is not null ? $"page {page}" : "")
				}{
					(totalPages is not null ? $" / {totalPages}" : "")
				}")
                .Build()
            );
        }
        protected async Task RespondInfo(string header, string message) {
            await Respond(PamelloEmbedBuilder.BuildInfo(header, message));
        }

        //general
        public async Task Ping()
        {
            await RespondInfo("Pong");
        }

        public async Task Connect() {
            if (!await Commands.SpeakerConnect()) {
                await RespondInfo("Cant connect");
                return;
            }

            await RespondInfo("Connected");
        }

        public async Task GetCode()
        {
            await RespondInfo("Authrorization Code", _authorization.GetCode(Context.User.DiscordUser.Id).ToString());
        }

        //Player
        public async Task PlayerSelect(string playerValue)
        {
            if (playerValue == "0") {
                await Commands.PlayerSelect(null);
                await RespondInfo("Select Player", $"Player selection reseted");
                return;
            }

            var player = _players.GetByValue(playerValue);
            if (player is null) throw new PamelloException($"Cant find a player with value \"{playerValue}\"");

            await Commands.PlayerSelect(player?.Id);

            await RespondInfo("Select Player", $"Player `{player}` selected");
        }
        public async Task PlayerCreate(string name)
        {
            var playerId = await Commands.PlayerCreate(name);
            await Commands.PlayerSelect(playerId);

            var player = _players.GetRequired(playerId);
            await RespondInfo("Select Player", $"Player `{player}` created and selected");
        }
        public async Task PlayerDelete(string playerValue)
        {
            throw new NotImplementedException();
        }

        public async Task PlayerResume() {
            await Commands.PlayerResume();
            await RespondInfo("Resumed");
        }
        public async Task PlayerPause() {
            await Commands.PlayerPause();
            await RespondInfo("Paused");
        }

        public async Task PlayerSkip()
        {
            var songId = await Commands.PlayerSkip();
            if (songId is null) throw new Exception("Unexpected error with song id being null ocurred");

            var song = _songs.GetRequired(songId.Value);

            await RespondInfo("Skip", $"Song `{song}` skipped");
        }
        public async Task PlayerGoTo(int songPosition, bool returnBack)
        {
            var songId = await Commands.PlayerGoTo(songPosition, returnBack);
            var song = _songs.GetRequired(songId);

            await RespondInfo("Go To", $"Playing `{song}`");
        }
        public async Task PlayerNext()
        {
            var songId = await Commands.PlayerNext();
            var song = _songs.GetRequired(songId);

            await RespondInfo("Next", $"Playing `{song}`");
        }
        public async Task PlayerPrev()
        {
            var songId = await Commands.PlayerPrev();
            var song = _songs.GetRequired(songId);

            await RespondInfo("Previous", $"Playing `{song}`");
        }
        public async Task PlayerGoToEpisode(int episodePosition)
        {
            await Commands.PlayerGoToEpisode(episodePosition);
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondInfo("Go To Episode", $"Playing `{episode}`");
        }
        public async Task PlayerNextEpisode()
        {
            await Commands.PlayerNextEpisode();
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondInfo("Next Episode", $"Playing `{episode}`");
        }
        public async Task PlayerPrevEpisode()
        {
            await Commands.PlayerPrevEpisode();
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondInfo("Previous Episode", $"Playing `{episode}`");
        }
        public async Task PlayerRewind(string strTime)
        {
            var time = AudioTime.FromStrTime(strTime);
            if (time is null) throw new PamelloException("Wrong time format");
            
            await Commands.PlayerRewind(time.Value.TotalSeconds);

            await RespondInfo("Player Rewind", $"Rewinded to `{time}`");
        }

        public async Task PlayerQueueSongAdd(string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongAdd(song.Id);

            await RespondInfo("Add song to the queue", $"Added `{song}`");
        }
        public async Task PlayerQueueSongInsert(int position, string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongInsert(position, song.Id);

            await RespondInfo("Insert song to the queue", $"Added `{song}`");
        }
        public async Task PlayerQueuePlaylistAdd(string playlistValue)
        {
            var playlist = _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueueSongAdd(playlist.Id);

            await RespondInfo("Add song to the queue", $"Added `{playlist}`");
        }
        public async Task PlayerQueuePlaylistInsert(int position, string playlistValue)
        {
            var playlist = _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueuePlaylistInsert(position, playlist.Id);

            await RespondInfo("Add song to the queue", $"Added `{playlist}`");
        }
        public async Task PlayerQueueSongRemove(int position)
        {
            var songId = await Commands.PlayerQueueSongRemove(position);

            var song = _songs.GetRequired(songId);
            await RespondInfo("Remove song from queue", $"Removed `{song}`");
        }
        public async Task PlayerQueueSongMove(int fromPosition, int toPosition)
        {
            await Commands.PlayerQueueSongMove(fromPosition, toPosition);

            await RespondInfo("Move songs", $"Moved song from position `{fromPosition}` to `{toPosition}`");
        }
        public async Task PlayerQueueSongSwap(int inPosition, int withPosition)
        {
            await Commands.PlayerQueueSongMove(inPosition, withPosition);

            await RespondInfo("Swap songs", $"Swaped song in position `{inPosition}` with `{withPosition}`");
        }
        public async Task PlayerQueueSongRequestNext(int? position)
        {
            await Commands.PlayerQueueSongRequestNext(position);

            if (Player.Queue.NextPositionRequest is null) {
                await RespondInfo("Next song request", "Next song will be played according to queue mode");
            }
            else {
                await RespondInfo("Next song request", $"`{Player.Queue.At(Player.Queue.NextPositionRequest.Value)}` will be played next");
            }
        }

        public async Task PlayerQueueRandom()
        {
            await Commands.PlayerQueueRandom(!Player.Queue.IsRandom);

            await RespondInfo("Random", (Player.Queue.IsRandom ? "Enabled" : "Disabled"));
        }
        public async Task PlayerQueueReversed()
        {
            await Commands.PlayerQueueReversed(!Player.Queue.IsReversed);

            await RespondInfo("Reversed", (Player.Queue.IsReversed ? "Enabled" : "Disabled"));
        }
        public async Task PlayerQueueNoLeftovers()
        {
            await Commands.PlayerQueueNoLeftovers(!Player.Queue.IsNoLeftovers);

            await RespondInfo("No Leftovers", (Player.Queue.IsNoLeftovers ? "Enabled" : "Disabled"));
        }

        public async Task PlayerQueueList(int page) {
            var pageContent = Player.Queue.GetQueuePage(page, 20);

            await RespondPage("Queue", pageContent, page, Player.Queue.Count / 20 + (Player.Queue.Count % 20 != 0 ? 1 : 0));
        }
        public async Task PlayerQueueSuffle()
        {
            await Commands.PlayerQueueSuffle();

            await RespondInfo("Queue suffled");
        }
        public async Task PlayerQueueClear()
        {
            await Commands.PlayerQueueClear();

            await RespondInfo("Queue cleared");
        }

        //song
        public async Task SongAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongSearch()
        {
            throw new NotImplementedException();
        }
        public async Task SongInfo()
        {
            throw new NotImplementedException();
        }
        public async Task SongRename()
        {
            throw new NotImplementedException();
        }

        public async Task SongFavoriteAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongFavoriteRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongFavoriteList()
        {
            throw new NotImplementedException();
        }

        public async Task SongAssociacionsAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongAssociacionsRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongAssociacionsList()
        {
            throw new NotImplementedException();
        }

        public async Task SongEpisodesAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesRename()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesClear()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesList()
        {
            throw new NotImplementedException();
        }

        //playlist
        public async Task PlaylistCreate()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistAddSong()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistAddPlaylistSongs()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistSearch()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistInfo()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistRename()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteAdd()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteRemove()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteList()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistDelete()
        {
            throw new NotImplementedException();
        }
    }
}
