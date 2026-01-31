using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.Core;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Modules.Discord.Base
{
    public class PamelloInteractionModuleBase : InteractionModuleBase<PamelloSocketInteractionContext>
    {
        private readonly ICodeAuthorizationService _authorization;
        private readonly DiscordClientService _discordClients;

        private readonly IPamelloPlayerRepository _players;
        private readonly IPamelloSpeakerRepository _speakers;

        private readonly IPamelloUserRepository _users;
        private readonly IPamelloSongRepository _songs;
        private readonly IPamelloEpisodeRepository _episodes;
        private readonly IPamelloPlaylistRepository _playlists;

        private IPamelloCommandsModule Commands {
            get => throw new NotImplementedException(); //Context.User.Commands;
        }
        private IPamelloPlayer Player {
            get => Context.User.RequiredSelectedPlayer;
        }
        private IPamelloPlayer RequiredPlayer {
            get => Context.User.SelectedPlayer ?? throw new PamelloException("Selected player is required for this command");
        }

        public PamelloInteractionModuleBase(IServiceProvider services)
        {
            _authorization = services.GetRequiredService<ICodeAuthorizationService>();
            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<IPamelloPlayerRepository>();
            _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();

            _users = services.GetRequiredService<IPamelloUserRepository>();
            _songs = services.GetRequiredService<IPamelloSongRepository>();
            _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
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

            /*
            var guild = Context.Guild;
            var guildUser = guild.GetUser(Context.User.DiscordId); //DISCORDUSER
            var vc = guildUser.VoiceChannel;

            if (vc is null) return;

            var users = _discordClients.GetVoiceChannelUsers(vc);

            Console.WriteLine("vc usres:");
            foreach (var user in users) {
                Console.WriteLine(user);
            }
            */
        }

        public async Task GetCode()
        {
            await RespondInfo("Authrorization Code", _authorization.GetCode(Context.User).ToString());
        }
        public async Task GetClient()
        {
            await RespondInfo("Pamello Client", 
                """
                You can download the client from [github releases](https://surl.lu/cyfdca) page
                - [Windows](https://surl.lu/cyfdca)
                - [Linux](https://surl.lu/cyfdca)
                - [Android](https://surl.lu/cyfdca)
                """
            );
        }

        //Player
        public async Task PlayerSelect(string playerValue)
        {
            var player = await _players.GetByValue(playerValue, Context.User);

            await Commands.PlayerSelect(player);

            if (player is null) {
                await RespondInfo($"Player with value {DiscordString.Code(playerValue)} not found, selection reseted");
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

        public async Task PlayerSearch(string query, int page)
        {
            var results = await _players.SearchAsync(query, Context.User);

            await RespondPage(
                query.Length == 0 ? "Players" : $"Players search \"{query}\"",
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
            var song = await Commands.PlayerSkip();
            if (song is null) throw new Exception("Unexpected error with song id being null ocurred");

            await RespondPlayerInfo("Skip", $"Song {song.ToDiscordString()} skipped");
        }
        public async Task PlayerGoTo(int songPosition, bool returnBack)
        {
            var song = await Commands.PlayerGoTo(songPosition, returnBack);
            
            await RespondPlayerInfo("Go To", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerNext()
        {
            var song = await Commands.PlayerNext();

            await RespondPlayerInfo("Next", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerPrev()
        {
            var song = await Commands.PlayerPrev();

            await RespondPlayerInfo("Previous", $"Playing {song.ToDiscordString()}");
        }
        public async Task PlayerGoToEpisode(int episodePosition)
        {
            await Commands.PlayerGoToEpisode(episodePosition);
            var episode = Player.Queue.Audio?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Go To Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerNextEpisode()
        {
            await Commands.PlayerNextEpisode();
            var episode = Player.Queue.Audio?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Next Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerPrevEpisode()
        {
            await Commands.PlayerPrevEpisode();
            var episode = Player.Queue.Audio?.GetCurrentEpisode();
            if (episode is null) throw new Exception("Unexpected episode null exception");

            await RespondPlayerInfo("Previous Episode", $"Playing {episode.ToDiscordString()}");
        }
        public async Task PlayerRewind(string strTime)
        {
            var time = AudioTime.FromStrTime(strTime);
            
            await Commands.PlayerRewind(time.TotalSeconds);

            await RespondPlayerInfo("Player Rewind", $"Rewinded to {DiscordString.Code(time)}");
        }

        public async Task PlayerQueueSongAdd(string songValue, int? position)
        {
            var song = await _songs.GetByValue(songValue, Context.User);
            if (song is null) throw new PamelloException($"Cant get song by value \"{songValue}\"");

            await Commands.PlayerQueueSongAdd(song, position);

            await Respond(
                PamelloEmbedBuilder.Info("Add song to the queue", $"Added {song.ToDiscordString()}", Player.ToDiscordFooterString())
                    .WithThumbnailUrl(song.CoverUrl)
                    .Build()
            );
        }
        public async Task PlayerQueuePlaylistAdd(string playlistValue, int? position)
        {
            var playlist = await _playlists.GetByValue(playlistValue, Context.User);
            if (playlist is null) throw new PamelloException($"Cant get playlist by value \"{playlistValue}\"");

            await Commands.PlayerQueuePlaylistAdd(playlist, position);

            await RespondPlayerInfo("Add song to the queue", $"Added {playlist.ToDiscordString()}");
        }
        
        public async Task PlayerQueueSongRemove(int position)
        {
            var song = await Commands.PlayerQueueSongRemove(position);

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
            var pageBuilder = PamelloEmbedBuilder.QueuePage(Player.Queue, page - 1 ?? 0, 20);

            await Respond(pageBuilder.Build());
        }
        public async Task PlayerQueueSuffle()
        {
            await Commands.PlayerQueueShuffle();

            await RespondPlayerInfo("Queue suffled");
        }
        public async Task PlayerQueueClear()
        {
            await Commands.PlayerQueueClear();

            await RespondPlayerInfo("Queue cleared");
        }

        //song
        public async Task SongAdd(string youtubeUrl) {
            var youtubeId = ""; //_youtubeInfo.GetVideoIdFromUrl(youtubeUrl);

            var song = (IPamelloSong)null; //_songs.GetByYoutubeId(youtubeId);
            if (song is not null) {
                await RespondInfo("Song is already present in the database");
                return;
            }

            //song = await _songs.AddAsync(youtubeId, Context.User);
            if (song is null) throw new PamelloException("Cant add this song");

            await RespondPlayerInfo($"Song added", $"Song {song.ToDiscordString()} added to the database");
        }
        public async Task SongSearch(string querry, int page, SocketUser? addedByDiscordUser)
        {
            IPamelloUser? addedBy = null;
            if (addedByDiscordUser is not null) {
                addedBy = _users.GetByDiscord(addedByDiscordUser.Id, false);

                if (addedBy is null) {
                    throw new PamelloException($"User {new DiscordString(addedByDiscordUser)} is not a known pamello user");
                }
            }

            var results = _songs.Search(querry, addedBy);
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

            if (Context.User.FavoriteSongs.Contains(song)) {
                await RespondInfo($"{song.ToDiscordString()} is already favorite");;
                return;
            }

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
            /*
            IPamelloUser? targetUser = null;
            if (targetDiscordUser is not null && targetDiscordUser.Id != Context.User.DiscordId) { //DISCORDUSER
                targetUser = _users.GetByDiscord(targetDiscordUser.Id);
                if (targetUser is null) throw new Exception("Cant find a provided user");
            }
            if (targetUser is null) {
                targetUser = Context.User;
            }

            var results = _songs.Search(querry, Context.User, favoriteBy: targetUser);

            await RespondPage(
                targetUser.Id == Context.User.Id ? "Favorite songs" : $"Favorite songs of {targetUser.Name}",
                results,
                (sb, pos, song) => {
                    sb.AppendLine(song.ToDiscordString().ToString());
                },
                page - 1
            );
            */
        }

        public async Task SongAssociationsAdd(string songValue, string association)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            await Commands.SongAssociationsAdd(song, association);

            await RespondInfo($"Association \"{association}\" added to song {song.ToDiscordString()}");
        }
        public async Task SongAssociationsRemove(string songValue, string association)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            await Commands.SongAssociationsRemove(song, association);

            await RespondInfo($"Association \"{association}\" removed from song {song.ToDiscordString()}");
        }
        public async Task SongAssociationsList(string songValue, int page)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await RespondPage(
                $"Associations of song [{song.Id}]",
                song.Associations,
                (sb, pos, association) => {
                    sb.AppendLine(DiscordString.Code(association).ToString());
                },
                page - 1
            );
        }

        public async Task SongEpisodesAdd(string songValue, string episodeTime, string episodeName)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);
            var episodeStart = AudioTime.FromStrTime(episodeTime);

            var episode = await Commands.SongEpisodeAdd(song, episodeStart.TotalSeconds, episodeName);

            await RespondInfo($"Episode {episode.ToDiscordString()} added");
        }
        public async Task SongEpisodesRemove(string songValue, int episodePosition)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodeRemove(song, episodePosition);

            await RespondInfo($"Episode removed");
        }
        public async Task SongEpisodesRename(string songValue, int episodePosition, string newName)
        {
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.SongEpisodeRename(song, episodePosition, newName);

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

            await Commands.SongEpisodeSkipSet(song, episodePosition, state == EBoolState.Enabled);

            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) {
                await RespondInfo($"Episode skip");
                return;
            }

            if (episode.AutoSkip) {
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

            await Commands.SongEpisodeEditTime(song, episodePosition, newStart.TotalSeconds);

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
            var currentEpisode = await _episodes.GetByValue("current", Context.User);

            await RespondPage(
                $"Episodes of {song.Name} [{song.Id}]",
                song.Episodes,
                (sb, pos, episode) => {
                    if (episode == currentEpisode) {
                        sb.AppendLine(DiscordString.Bold(DiscordString.Code(pos) + " " + episode.ToDiscordString()).ToString());
                    }
                    else {
                        sb.AppendLine((DiscordString.Code(pos) + " " + episode.ToDiscordString()).ToString());
                    }
                },
                page - 1
            );
        }

        //playlist
        public async Task PlaylistCreate(string name, bool fillWithQueue)
        {
            var playlist = await Commands.PlaylistCreate(name, fillWithQueue);

            await RespondInfo($"Playlist {playlist.ToDiscordString()} created");
        }

        public async Task PlaylistAddSong(string playlistValue, string songValue, int? position)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            var song = await _songs.GetByValueRequired(songValue, Context.User);

            await Commands.PlaylistAddSong(playlist, song, position);

            await RespondInfo($"Added {song.ToDiscordString()} to the {playlist.ToDiscordString()}");
        }
        public async Task PlaylistAddPlaylistSongs(string toPlaylistValue, string fromPlaylistValue, int? position)
        {
            var fromPlaylist = await _playlists.GetByValueRequired(fromPlaylistValue, Context.User);
            var toPlaylist = await _playlists.GetByValueRequired(toPlaylistValue, Context.User);

            await Commands.PlaylistAddPlaylistSongs(toPlaylist, fromPlaylist, position);

            await RespondInfo($"Songs from {fromPlaylist.ToDiscordString()} added to the {toPlaylist.ToDiscordString()}");
        }
        
        public async Task PlaylistMoveSong(string playlistValue, int fromPosition, int toPosition)
        {
            if (fromPosition == toPosition) {
                throw new PamelloException("Target position should differ from starting position");
            }
            
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);

            var song = await Commands.PlaylistMoveSong(playlist, fromPosition, toPosition);
            if (song is null) throw new PamelloException("No song to move");

            await RespondInfo($"Songs {song.ToDiscordString()} moved from {DiscordString.Code(fromPosition)} to {DiscordString.Code(toPosition)}");
        }

        public async Task PlaylistRemoveSong(string playlistValue, string songValue) {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            var song = await _songs.GetByValueRequired(songValue, Commands.User);

            var count = await Commands.PlaylistRemoveSong(playlist, song);
            if (count == 0) throw new PamelloException($"Song {song.ToDiscordString()} not found in {playlist.ToDiscordString()}");

            await RespondInfo($"All {DiscordString.Code(count)} items of {song.ToDiscordString()} removed from {playlist.ToDiscordString()}");
        }
        
        public async Task PlaylistRemoveAt(string playlistValue, int position) {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);

            var song = await Commands.PlaylistRemoveAt(playlist, position);
            if (song is null) throw new PamelloException($"No song was found at {DiscordString.Code(position)} in {playlist.ToDiscordString()}");

            await RespondInfo($"{song.ToDiscordString()} at {DiscordString.Code(position)} removed from {playlist.ToDiscordString()}");
        }

        public async Task PlaylistSearch(string querry, int page, SocketUser? addedByDiscordUser)
        {
            IPamelloUser? addedBy = null;
            if (addedByDiscordUser is not null) {
                addedBy = _users.GetByDiscord(addedByDiscordUser.Id);
            }

            var results = _playlists.Search(querry, addedBy);
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
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);

            await RespondPage(
                $"Songs of playlist \"{playlist.Name}\"",
                playlist.Songs,
                (sb, pos, song) => {
                    sb.AppendLine((DiscordString.Code(pos) + " - " + song.ToDiscordString()).ToString());
                },
                page - 1
            );
        }
        public async Task PlaylistInfo(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            await Respond(PamelloEmbedBuilder.BuildPlaylistInfo(playlist));
        }
        public async Task PlaylistRename(string playlistValue, string newName)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            await Commands.PlaylistRename(playlist, newName);

            await RespondInfo($"Playlist {playlist.ToDiscordString()} renamed");
        }
        public async Task PlaylistFavoriteAdd(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);

            if (Context.User.FavoritePlaylists.Contains(playlist)) {
                await RespondInfo($"{playlist.ToDiscordString()} is already favorite");;
                return;
            }
            
            await Commands.PlaylistFavoriteAdd(playlist);

            await RespondInfo($"{playlist.ToDiscordString()} added to favorites");
        }
        public async Task PlaylistFavoriteRemove(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            await Commands.PlaylistFavoriteRemove(playlist);

            await RespondInfo($"{playlist.ToDiscordString()} removed from favorites");
        }
        public async Task PlaylistFavoriteList(string querry, int page, SocketUser? targetDiscordUser)
        {
            IPamelloUser? targetUser = null;
            /*
            if (targetDiscordUser is not null && targetDiscordUser.Id != Context.User.DiscordId) { //DISCORDUSER
                targetUser = _users.GetByDiscord(targetDiscordUser.Id);
                if (targetUser is null) throw new Exception("Cant find a provided user");
            }
            if (targetUser is null) {
                targetUser = Context.User;
            }

            var results = _playlists.Search(querry, Context.User, favoriteBy: targetUser);

            await RespondPage(
                targetUser.Id == Context.User.Id ? "Favorite playlists" : $"Favorite playlists of {targetUser.Name}",
                results,
                (sb, pos, playlist) => {
                    sb.AppendLine(playlist.ToDiscordString().ToString());
                },
                page - 1
            );
            */
        }
        public async Task PlaylistDelete(string playlistValue)
        {
            var playlist = await _playlists.GetByValueRequired(playlistValue, Context.User);
            await Commands.PlaylistDelete(playlist);
        }

        //speakers
        /*
        public async Task SpeakerDiscordConnect() {
            Context.User.TryLoadLastPlayer();

            var speaker = await Commands.SpeakerDiscordConnect();

            await RespondPlayerInfo("Connected", $"speaker {speaker.ToDiscordString()} connected to your vc");
        }
        */

        public async Task SpeakerInfo() {
            throw new NotImplementedException();
        }

        public async Task SpeakerDisconnect(string speakerValue)
        {
            var speaker = await _speakers.GetByValueRequired(speakerValue, Context.User);
            
            await Commands.SpeakerDisconnect(speaker);

            await RespondPlayerInfo("Disconnected", $"Speaker {speaker.ToDiscordString()} disconnected");
        }

        public async Task SpeakerConnectInternet(string? name) {
            var speaker = await Commands.SpeakerInternetConnect(name);
            
            await RespondPlayerInfo("Connected", $"Internet speaker {speaker.ToDiscordString()} connected");
        }

        public async Task SpeakerInternetRename(string speakerValue, string newName) {
            var speaker = await _speakers.GetByValueRequired(speakerValue, Context.User);
            if (speaker is not IPamelloInternetSpeaker internetSpeaker) throw new PamelloException($"Speaker {speaker.ToDiscordString()} is not internet speaker");
            
            await Commands.SpeakerInternetRename(internetSpeaker, newName);
            
            await RespondInfo($"Internet speaker {speaker.ToDiscordString()} renamed");
        }

        /*
        public async Task SpeakerSearch(string query, int page, ESpeakerType? type)
        {
            var results = type switch
            {
                ESpeakerType.Internet => await _speakers.SearchAsync<IPamelloInternetSpeaker>(query, Context.User),
                ESpeakerType.Discord => await _speakers.SearchAsync<PamelloDiscordSpeaker>(query, Context.User),
                _ => await _speakers.SearchAsync(query, Context.User)
            };

            await RespondPage(
                (query?.Length > 0 ? $"Search \"{query}\" in player" : "Player") + $" {RequiredPlayer} speakers" + (type is not null ? $" ({type} only)" : ""),
                results,
                (sb, pos, speaker) =>
                    sb.AppendLine(speaker.ToDiscordString().ToString()),
                page
            );
        }
        */
    }
}
