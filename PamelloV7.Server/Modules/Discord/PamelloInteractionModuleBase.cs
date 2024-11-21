using Discord;
using Discord.Interactions;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Discord;
using System.Text;

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
        protected async Task RespondPlayerInfo(object message, bool bold = true) {
            if (bold) await RespondPlayerInfo(message, "");
            else await RespondPlayerInfo("", message);
        }
        protected async Task RespondPlayerInfo(object header, object message) {
            await Respond(PamelloEmbedBuilder.BuildInfo(header.ToString() ?? "", message.ToString() ?? "", $"Selected player: {Player.Name} [{Player.Id}]"));
        }
        protected async Task RespondInfo(object message, bool bold = false) {
            if (bold) await RespondInfo(message, "");
            else await RespondInfo("", message);
        }
        protected async Task RespondInfo(object header, object message) {
            await Respond(PamelloEmbedBuilder.BuildInfo(header.ToString() ?? "", message.ToString() ?? ""));
        }
        protected async Task RespondPage<T>(string header, List<T> content, Action<StringBuilder, int, T> tostr, int page = 0, int count = 20) {
            await Respond(PamelloEmbedBuilder.BuildPage(header, content, tostr, count, page));
        }

        //general
        public async Task Ping()
        {
            await RespondInfo("Pong");
        }

        public async Task Connect() {
            Context.User.TryLoadLastPlayer();

            if (!await Commands.SpeakerConnect()) {
                await RespondPlayerInfo("Cant connect");
                return;
            }

            await RespondPlayerInfo("Connected");
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

            await RespondInfo("Select Player", $"Player {player?.ToDiscordString()} selected");
        }
        public async Task PlayerCreate(string name)
        {
            var playerId = await Commands.PlayerCreate(name);
            await Commands.PlayerSelect(playerId);

            var player = _players.GetRequired(playerId);
            await RespondInfo("Select Player", $"Player {player?.ToDiscordString()} created and selected");
        }
        public async Task PlayerList(string querry, int page)
        {
            var results = _players.Search(querry, Context.User);

            await RespondPage(
                "Players",
                results,
                (sb, pos, player) => sb.AppendLine(player.ToDiscordString().ToString()),
                page - 1
            );
        }
        public async Task PlayerDelete(string playerValue)
        {
            throw new NotImplementedException();
        }

        public async Task PlayerResume() {
            await Commands.PlayerResume();
            await RespondPlayerInfo("Resumed");
        }
        public async Task PlayerPause() {
            await Commands.PlayerPause();
            await RespondPlayerInfo("Paused");
        }

        public async Task PlayerSkip()
        {
            var songId = await Commands.PlayerSkip();
            if (songId is null) throw new Exception("Unexpected error with song id being null ocurred");

            var song = _songs.GetRequired(songId.Value);

            await RespondPlayerInfo("Skip", $"Song {song.ToDiscordString()} skipped");
        }
        public async Task PlayerGoTo(int songPosition, bool returnBack)
        {
            var songId = await Commands.PlayerGoTo(songPosition, returnBack);
            var song = _songs.GetRequired(songId);

            await RespondPlayerInfo("Go To", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerNext()
        {
            var songId = await Commands.PlayerNext();
            var song = _songs.GetRequired(songId);

            await RespondPlayerInfo("Next", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerPrev()
        {
            var songId = await Commands.PlayerPrev();
            var song = _songs.GetRequired(songId);

            await RespondPlayerInfo("Previous", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerGoToEpisode(int episodePosition)
        {
            await Commands.PlayerGoToEpisode(episodePosition);
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Go To Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerNextEpisode()
        {
            await Commands.PlayerNextEpisode();
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Next Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerPrevEpisode()
        {
            await Commands.PlayerPrevEpisode();
            var episode = Player.Queue.Current?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Previous Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerRewind(string strTime)
        {
            var time = AudioTime.FromStrTime(strTime);
            if (time is null) throw new PamelloException("Wrong time format");
            
            await Commands.PlayerRewind(time.Value.TotalSeconds);

            await RespondPlayerInfo("Player Rewind", $"Rewinded to {DiscordString.Code(time)}");
        }

        public async Task PlayerQueueSongAdd(string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongAdd(song.Id);

            await RespondPlayerInfo("Add song to the queue", $"Added {song.ToDiscordString()}");
        }
        public async Task PlayerQueueSongInsert(int position, string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongInsert(position, song.Id);

            await RespondPlayerInfo("Insert song to the queue", $"Added {song.ToDiscordString()}");
        }
        public async Task PlayerQueuePlaylistAdd(string playlistValue)
        {
            var playlist = _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueueSongAdd(playlist.Id);

            await RespondPlayerInfo("Add song to the queue", $"Added {playlist.ToDiscordString()}");
        }
        public async Task PlayerQueuePlaylistInsert(int position, string playlistValue)
        {
            var playlist = _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueuePlaylistInsert(position, playlist.Id);

            await RespondPlayerInfo("Add song to the queue", $"Added {playlist.ToDiscordString()}");
        }
        public async Task PlayerQueueSongRemove(int position)
        {
            var songId = await Commands.PlayerQueueSongRemove(position);

            var song = _songs.GetRequired(songId);
            await RespondPlayerInfo("Remove song from queue", $"Removed {song.ToDiscordString()}");
        }
        public async Task PlayerQueueSongMove(int fromPosition, int toPosition)
        {
            await Commands.PlayerQueueSongMove(fromPosition, toPosition);

            await RespondPlayerInfo("Move songs", $"Moved song from position `{fromPosition}` to `{toPosition}`");
        }
        public async Task PlayerQueueSongSwap(int inPosition, int withPosition)
        {
            await Commands.PlayerQueueSongMove(inPosition, withPosition);

            await RespondPlayerInfo("Swap songs", $"Swaped song in position `{inPosition}` with `{withPosition}`");
        }
        public async Task PlayerQueueSongRequestNext(int? position)
        {
            await Commands.PlayerQueueSongRequestNext(position);

            if (Player.Queue.NextPositionRequest is null) {
                await RespondPlayerInfo("Next song request", "Next song will be played according to queue mode");
            }
            else {
                await RespondPlayerInfo("Next song request", $"{Player.Queue.At(Player.Queue.NextPositionRequest.Value)?.ToDiscordString()} will be played next");
            }
        }

        public async Task RespondQueueMode() {
            await RespondPlayerInfo("Queue modes", $@"Random: {DiscordString.Code(Player.Queue.IsRandom ? "Enabled" : "Disabled")}
Reversed: {DiscordString.Code(Player.Queue.IsReversed ? "Enabled" : "Disabled")}
No Leftoves: {DiscordString.Code(Player.Queue.IsNoLeftovers ? "Enabled" : "Disabled")}
Feed Random: {DiscordString.Code(Player.Queue.IsFeedRandom ? "Enabled" : "Disabled")}");
        }
        public async Task PlayerQueueRandom(EBoolState state)
        {
            await Commands.PlayerQueueRandom(state == EBoolState.Enabled);
            await RespondQueueMode();
        }
        public async Task PlayerQueueReversed(EBoolState state)
        {
            await Commands.PlayerQueueReversed(state == EBoolState.Enabled);
            await RespondQueueMode();
        }
        public async Task PlayerQueueNoLeftovers(EBoolState state)
        {
            await Commands.PlayerQueueNoLeftovers(state == EBoolState.Enabled);
            await RespondQueueMode();
        }
        public async Task PlayerQueueFeedRandom(EBoolState state)
        {
            await Commands.PlayerQueueFeedRandom(state == EBoolState.Enabled);
            await RespondQueueMode();
        }

        public async Task PlayerQueueList(int? page) {
            var pageBuilder = Player.Queue.QueuePageBuilder(page ?? 0, 20);

            await Respond(pageBuilder.Build());
        }
        public async Task PlayerQueueSuffle()
        {
            await Commands.PlayerQueueSuffle();

            await RespondPlayerInfo("Queue suffled");
        }
        public async Task PlayerQueueClear()
        {
            await Commands.PlayerQueueClear();

            await RespondPlayerInfo("Queue cleared");
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
