using Discord;
using Discord.Interactions;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Discord;
using System.Text;
using Discord.WebSocket;

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

        private readonly YoutubeInfoService _youtubeInfo;

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

            _youtubeInfo = services.GetRequiredService<YoutubeInfoService>();
        }

        protected async Task Respond(Embed embed) {
            await ModifyOriginalResponseAsync(message => message.Embed = embed);
        }
        protected async Task RespondPlayerInfo(object message, bool bold = true) {
            if (bold) await RespondPlayerInfo(message, "");
            else await RespondPlayerInfo("", message);
        }
        protected async Task RespondPlayerInfo(object header, object message) {
            await Respond(PamelloEmbedBuilder.BuildInfo(header.ToString() ?? "", message.ToString() ?? "", Player.ToDiscordFooterString()));
        }
        protected async Task RespondInfo(object message, bool bold = false) {
            if (bold) await RespondInfo(message, "");
            else await RespondInfo("", message);
        }
        protected async Task RespondInfo(object header, object message) {
            await Respond(PamelloEmbedBuilder.BuildInfo(header.ToString() ?? "", message.ToString() ?? ""));
        }
        protected async Task RespondPage<T>(string header, IEnumerable<T> content, Action<StringBuilder, int, T> writeElement, int page = 0, int count = 20) {
            await Respond(PamelloEmbedBuilder.BuildPage(header, content, writeElement, count, page));
        }

        //general
        public async Task Ping()
        {
            await RespondInfo("Pong");

            var guild = Context.Guild;
            var guildUser = guild.GetUser(Context.User.DiscordId);
            var vc = guildUser.VoiceChannel;

            if (vc is null) return;

            var users = _discordClients.GetVoiceChannelUsers(vc);

            Console.WriteLine("vc usres:");
            foreach (var user in users) {
                Console.WriteLine(user);
            }
        }

        public async Task GetCode()
        {
            await RespondInfo("Authrorization Code", _authorization.GetCode(Context.User.DiscordId).ToString());
        }

        //Player
        public async Task PlayerSelect(string playerValue)
        {
            var player = await _players.GetByValue(playerValue);
            if (player is null) throw new PamelloException($"Cant find a player with value \"{playerValue}\"");

            await Commands.PlayerSelect(player);

            if (Player is null) {
                await RespondInfo($"Player selection reseted");
            }
            else {
                await RespondInfo($"Player {player?.ToDiscordString()} selected");
            }
        }
        public async Task PlayerCreate(string name)
        {
            var player = await Commands.PlayerCreate(name);
            await Commands.PlayerSelect(player);

            await RespondInfo($"Player {player?.ToDiscordString()} created and selected");
        }
        public async Task PlayerProtection(EBoolState state)
        {
            await Commands.PlayerProtection(state == EBoolState.Enabled);

            await RespondInfo($"Player protection {DiscordString.Code(state)}");
        }

        public async Task PlayerInfo() {
            await Respond(PamelloEmbedBuilder.BuildPlayerInfo(Player));
        }

        public async Task PlayerList(string querry, int page)
        {
            var results = await _players.Search(querry, Context.User);

            await RespondPage(
                querry.Length == 0 ? "Players" : $"Players search \"{querry}\"",
                results,
                (sb, pos, player) => sb.AppendLine((player.ToDiscordString() + (player.IsProtected ? " " + DiscordString.Italic("(pivate)") : new DiscordString(""))).ToString()),
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
            
            await Commands.PlayerRewind(time.TotalSeconds);

            await RespondPlayerInfo("Player Rewind", $"Rewinded to {DiscordString.Code(time)}");
        }

        public async Task PlayerQueueSongAdd(string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongAdd(song);

            await RespondPlayerInfo("Add song to the queue", $"Added {song.ToDiscordString()}");
        }
        public async Task PlayerQueueSongInsert(int position, string songValue)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongInsert(position, song);

            await RespondPlayerInfo("Insert song to the queue", $"Added {song.ToDiscordString()}");
        }
        public async Task PlayerQueuePlaylistAdd(string playlistValue)
        {
            var playlist = await _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueuePlaylistAdd(playlist);

            await RespondPlayerInfo("Add song to the queue", $"Added {playlist.ToDiscordString()}");
        }
        public async Task PlayerQueuePlaylistInsert(int position, string playlistValue)
        {
            var playlist = await _playlists.GetByValue(playlistValue);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueuePlaylistInsert(position, playlist);

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
                await RespondPlayerInfo("Next song request", $"{Player.Queue.SongAt(Player.Queue.NextPositionRequest.Value)?.ToDiscordString()} will be played next");
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
            var pageBuilder = Player.Queue.QueuePageBuilder(page - 1 ?? 0, 20);

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
        public async Task SongAdd(string youtubeUrl)
        {
            var youtubeId = _youtubeInfo.GetVideoIdFromUrl(youtubeUrl);

            var song = _songs.GetByYoutubeId(youtubeId);
            if (song is not null) {
                await RespondInfo("Song is already present in the database");
                return;
            }

            song = await _songs.AddAsync(youtubeId, Context.User);
            if (song is null) throw new PamelloException("Cant add this song");

            await RespondPlayerInfo($"{song.ToDiscordString()} added to database");
        }
        public async Task SongSearch(string querry, int page, SocketUser? addedByDiscordUser)
        {
            PamelloUser? addedBy = null;
            if (addedByDiscordUser is not null) {
                addedBy = _users.GetByDiscord(addedByDiscordUser.Id);
            }

            var results = await _songs.Search(querry, addedBy);
            string title;

            if (querry.Length == 0) {
                if (addedBy is null) {
                    title = "Songs";
                }
                else {
                    title = $"Songs added by {addedBy.Name}";
                }
            }
            else {
                if (addedBy is null) {
                    title = $"Songs search \"{querry}\"";
                }
                else {
                    title = $"Songs added by {addedBy.Name} search \"{querry}\"";
                }
            }

            await RespondPage(
                title,
                results,
                (sb, pos, song) => {
                    sb.AppendLine(song.ToDiscordString().ToString());
                },
                page - 1
            );
        }
        public async Task SongInfo(string songValue)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            await Respond(PamelloEmbedBuilder.BuildSongInfo(song));
        }
        public async Task SongRename(string songValue, string newName)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant find song by value \"{songValue}\"");

            await Commands.SongRename(song, newName);

            await RespondInfo($"{song.ToDiscordString()} renamed");
        }

        public async Task SongFavoriteAdd(string songValue)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongFavoriteAdd(song);

            await RespondInfo($"{song.ToDiscordString()} added to favorites");
        }
        public async Task SongFavoriteRemove(string songValue)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongFavoriteRemove(song);

            await RespondInfo($"{song.ToDiscordString()} removed from favorites");
        }
        public async Task SongFavoriteList(string querry, int page, SocketUser? targetDiscordUser)
        {
            PamelloUser? targetUser = null;
            if (targetDiscordUser is not null && targetDiscordUser.Id != Context.User.DiscordId) {
                targetUser = _users.GetByDiscord(targetDiscordUser.Id);
                if (targetUser is null) throw new Exception("Cant find a provided user");
            }
            if (targetUser is null) {
                targetUser = Context.User;
            }

            var results = await _songs.Search(querry, favoriteBy: targetUser);

            await RespondPage(
                targetUser.Id == Context.User.Id ? "Favorite songs" : $"Favorite songs of {targetUser.Name}",
                results,
                (sb, pos, song) => {
                    sb.AppendLine(song.ToDiscordString().ToString());
                },
                page - 1
            );
        }

        public async Task SongAssociacionsAdd(string songValue, string associacion)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            await Commands.SongAssociacionsAdd(song, associacion);

            await RespondInfo($"Associacion \"{associacion}\" added to song {song.ToDiscordString()}");
        }
        public async Task SongAssociacionsRemove(string songValue, string associacion)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            await Commands.SongAssociacionsRemove(song, associacion);

            await RespondInfo($"Associacion \"{associacion}\" removed from song {song.ToDiscordString()}");
        }
        public async Task SongAssociacionsList(string songValue, int page)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await RespondPage(
                $"Associacions of song [{song.Id}]",
                song.Associacions,
                (sb, pos, associacion) => {
                    sb.AppendLine(DiscordString.Code(associacion).ToString());
                },
                page - 1
            );
        }

        public async Task SongEpisodesAdd(string songValue, string episodeTime, string episodeName)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            var episodeStart = AudioTime.FromStrTime(episodeTime);

            var episodeId = await Commands.SongEpisodesAdd(song, episodeStart.TotalSeconds, episodeName);
            var episode = _episodes.GetRequired(episodeId);

            await RespondInfo($"Episode {episode.ToDiscordString()} added");
        }
        public async Task SongEpisodesRemove(string songValue, int episodePosition)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodesRemove(song, episodePosition);

            await RespondInfo($"Episode removed");
        }
        public async Task SongEpisodesRename(string songValue, int episodePosition, string newName)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodesRename(song, episodePosition, newName);

            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) {
                await RespondInfo($"Episode renamed");
                return;
            }

            await RespondInfo($"Episode {episode.ToDiscordString()} renamed");
        }
        public async Task SongEpisodesSkipSet(string songValue, int episodePosition, EBoolState state)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodesSkipSet(song, episodePosition, state == EBoolState.Enabled);

            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) {
                await RespondInfo($"Episode skip");
                return;
            }

            if (episode.Skip) {
                await RespondInfo($"Episode {episode.ToDiscordString()} will be skipped");
            }
            else {
                await RespondInfo($"Episode {episode.ToDiscordString()} will be played");
            }
        }
        public async Task SongEpisodesChangeStart(string songValue, int episodePosition, string newTime)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            var newStart = AudioTime.FromStrTime(newTime);

            await Commands.SongEpisodesEditTime(song, episodePosition, newStart.TotalSeconds);

            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) {
                await RespondInfo($"Episode renamed");
                return;
            }

            await RespondInfo($"Episode {episode.ToDiscordString()} start position changed");
        }
        public async Task SongEpisodesClear(string songValue)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodesClear(song);

            await RespondInfo("Song episodes cleared");
        }
        public async Task SongEpisodesList(string songValue, int page)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await RespondPage(
                $"Episodes of song [{song.Id}]",
                song.Episodes,
                (sb, pos, episode) => {
                    sb.AppendLine((DiscordString.Code(pos) + " " + episode.ToDiscordString()).ToString());
                },
                page - 1
            );
        }

        //playlist
        public async Task PlaylistCreate(string name, bool fillWithQueue)
        {
            var playlistId = await Commands.PlaylistCreate(name, fillWithQueue);
            var playlist = _playlists.GetRequired(playlistId);

            await RespondInfo($"Playlist {playlist.ToDiscordString()} created");
        }

        public async Task PlaylistAddSong(string playlistValue, string songValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            var song = await _songs.GetByValueRequired(songValue);

            await Commands.PlaylistAddSong(playlist, song);

            await RespondInfo($"Added {song.ToDiscordString()} to the {playlist.ToDiscordString()}");
        }
        public async Task PlaylistAddPlaylistSongs(string toPlaylistValue, string fromPlaylistValue)
        {
            var fromPlaylist = await _playlists.GetByValueRequired(fromPlaylistValue);
            var toPlaylist = await _playlists.GetByValueRequired(toPlaylistValue);

            int count = await Commands.PlaylistAddPlaylistSongs(toPlaylist.Id, fromPlaylist.Id);

            await RespondInfo($"{count} songs added to the {toPlaylist.ToDiscordString()}");
        }

        public async Task PlaylistRemoveSong(string playlistValue, string songValue) {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            var song = await _songs.GetByValueRequired(songValue);

            await Commands.PlaylistRemoveSong(playlist, song);

            await RespondInfo($"{song.ToDiscordString()} removed from {playlist.ToDiscordString()}");
        }

        public async Task PlaylistSearch(string querry, int page, SocketUser? addedByDiscordUser)
        {
            PamelloUser? addedBy = null;
            if (addedByDiscordUser is not null) {
                addedBy = _users.GetByDiscord(addedByDiscordUser.Id);
            }

            var results = await _playlists.Search(querry, addedBy);
            string title;

            if (querry.Length == 0) {
                if (addedBy is null) {
                    title = "Playlists";
                }
                else {
                    title = $"Playlists added by {addedBy.Name}";
                }
            }
            else {
                if (addedBy is null) {
                    title = $"Playlists search \"{querry}\"";
                }
                else {
                    title = $"Playlists added by {addedBy.Name} search \"{querry}\"";
                }
            }

            await RespondPage(
                title,
                results,
                (sb, pos, playlist) => {
                    sb.AppendLine(playlist.ToDiscordString().ToString());
                },
                page - 1
            );
        }
        public async Task PlaylistSongsList(string playlistValue, int page)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);

            await RespondPage(
                $"Songs of playlist \"{playlist.Name}\"",
                playlist.Songs,
                (sb, pos, song) => {
                    sb.AppendLine(song.ToDiscordString().ToString());
                },
                page - 1
            );
        }
        public async Task PlaylistInfo(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            await Respond(PamelloEmbedBuilder.BuildPlaylistInfo(playlist));
        }
        public async Task PlaylistRename(string playlistValue, string newName)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            await Commands.PlaylistRename(playlist, newName);
        }
        public async Task PlaylistFavoriteAdd(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            await Commands.PlaylistFavoriteAdd(playlist);

            await RespondInfo($"{playlist.ToDiscordString()} added to favorites");
        }
        public async Task PlaylistFavoriteRemove(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            await Commands.PlaylistFavoriteRemove(playlist);

            await RespondInfo($"{playlist.ToDiscordString()} removed from favorites");
        }
        public async Task PlaylistFavoriteList(string querry, int page, SocketUser? targetDiscordUser)
        {
            PamelloUser? targetUser = null;
            if (targetDiscordUser is not null && targetDiscordUser.Id != Context.User.DiscordId) {
                targetUser = _users.GetByDiscord(targetDiscordUser.Id);
                if (targetUser is null) throw new Exception("Cant find a provided user");
            }
            if (targetUser is null) {
                targetUser = Context.User;
            }

            var results = await _playlists.Search(querry, favoriteBy: targetUser);

            await RespondPage(
                targetUser.Id == Context.User.Id ? "Favorite playlists" : $"Favorite playlists of {targetUser.Name}",
                results,
                (sb, pos, playlist) => {
                    sb.AppendLine(playlist.ToDiscordString().ToString());
                },
                page - 1
            );
        }
        public async Task PlaylistDelete(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue);
            await Commands.PlaylistDelete(playlist);
        }

        //speakers
        public async Task SpeakerConnect() {
            Context.User.TryLoadLastPlayer();

            await Commands.SpeakerConnect();

            await RespondPlayerInfo("Connected");
        }

        public async Task SpeakerDisconnect() {
            await Commands.SpeakerDisconnect();

            await RespondPlayerInfo("Disconnected");
        }

        public async Task SpeakerList() {
        }
    }
}
