using PamelloV7.Server.Attributes;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Audio;
using PamelloV7.Server.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server.Modules
{
    public class PamelloCommandsModule
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly PamelloPlayerRepository _players;
        private readonly PamelloSpeakerRepository _speakers;

        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;

        public PamelloUser User { get; }
        private PamelloPlayer Player {
            get => User.RequiredSelectedPlayer;
        }

        public PamelloCommandsModule(IServiceProvider services, PamelloUser user) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<PamelloPlayerRepository>();
            _speakers = services.GetRequiredService<PamelloSpeakerRepository>();

            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();

            User = user;

            //WriteAllCommands();
        }

        private void WriteAllCommands() {
            var methods = typeof(PamelloCommandsModule).GetMethods();


            Console.WriteLine("Commnds:\n");
            foreach (var method in methods) {
                if (!method.CustomAttributes.Any(attribure => attribure.AttributeType == typeof(PamelloCommandAttribute))) continue;

                var parameters = method.GetParameters();

                Console.Write($"public async {method.ReturnType.ShortDisplayName()} {method.Name}(");

                foreach (var parameter in parameters) {
                    Console.Write($"{parameter.ParameterType.ShortDisplayName()} {parameter.Name}");
                }

                Console.Write($") {{\n\treturn await InvokeCommand<object>($\"{method.Name}&");

                var first = true;
                foreach (var parameter in parameters) {
                    if (!first) Console.Write("&");
                    else first = false;

                    Console.Write($"{parameter.Name}={{{parameter.Name}}}");
                }

                Console.WriteLine("\");\n}");
            }
        }

        //player
        [PamelloCommand]
        public async Task<PamelloPlayer> PlayerCreate(string playerName) {
            var player = _players.Create(User, playerName);
            User.SelectedPlayer = player;

            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) return player;

            var vcUsers = _discordClients.GetVoiceChannelUsers(vc);
            foreach (var user in vcUsers) {
                if (user.SelectedPlayer is null) {
                    user.SelectedPlayer = player;
                }
            }

            return player;
        }

        [PamelloCommand]
        public async Task<PamelloPlayer?> PlayerSelect(PamelloPlayer? player) {
            User.RequireSelectPlayer(player);
            
            return User.SelectedPlayer;
        }
        [PamelloCommand]
        public async Task<bool> PlayerProtection(bool state) {
            if (Player.Creator != User) {
                throw new PamelloException("Only creator of the player can change it protection");
            }

            return Player.IsProtected = state;
        }
        [PamelloCommand]
        public async Task PlayerDelete() {
            throw new NotImplementedException();
        }
        
        [PamelloCommand]
        public async Task PlayerResume() {
            Player.IsPaused = false;
        }
        [PamelloCommand]
        public async Task PlayerPause() {
            Player.IsPaused = true;
        }

        [PamelloCommand]
        public async Task<PamelloSong?> PlayerSkip() {
            var song = Player.Queue.Audio?.Song;

            Player.Queue.GoToNextSong();

            return song;
        }
        [PamelloCommand]
        public async Task<PamelloSong> PlayerGoTo(int songPosition, bool returnBack) {
            return Player.Queue.GoToSong(songPosition, returnBack);
        }
        [PamelloCommand]
        public async Task<PamelloSong> PlayerPrev() {
            return Player.Queue.GoToSong(Player.Queue.Position - 1);
        }
        [PamelloCommand]
        public async Task<PamelloSong> PlayerNext() {
            return Player.Queue.GoToSong(Player.Queue.Position + 1);
        }

        [PamelloCommand]
        public async Task<PamelloEpisode?> PlayerGoToEpisode(int episodePosition) {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");
            return await Player.Queue.Audio.RewindToEpisode(episodePosition);
        }

        [PamelloCommand]
        public async Task<PamelloEpisode?> PlayerPrevEpisode() {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Audio.GetCurrentEpisodePosition() ?? 0;
            return await Player.Queue.Audio.RewindToEpisode(currentEpisode - 1, false);
        }

        [PamelloCommand]
        public async Task<PamelloEpisode?> PlayerNextEpisode() {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Audio.GetCurrentEpisodePosition() ?? 0;
            return await Player.Queue.Audio.RewindToEpisode(currentEpisode + 1, false);
        }

        [PamelloCommand]
        public async Task PlayerRewind(int seconds) {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");
            await Player.Queue.Audio.RewindTo(new AudioTime(seconds));
        }

        [PamelloCommand]
        public async Task<PamelloSong> PlayerQueueSongAdd(PamelloSong song, int? position = null) {
            return position is null ?
                Player.Queue.AddSong(song, User) :
                Player.Queue.InsertSong(position.Value, song, User);
        }
        
        [PamelloCommand]
        public async Task<PamelloPlaylist> PlayerQueuePlaylistAdd(PamelloPlaylist playlist, int? position = null) {
            return position is null ?
                Player.Queue.AddPlaylist(playlist, User) :
                Player.Queue.InsertPlaylist(position.Value, playlist, User);
        }
        
        [PamelloCommand]
        public async Task<PamelloSong> PlayerQueueSongRemove(int position) {
            return Player.Queue.RemoveSong(position);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongSwap(int inPosition, int withPosition) {
            Player.Queue.SwapSongs(inPosition, withPosition);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongMove(int fromPosition, int toPosition) {
            Player.Queue.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongRequestNext(int? position) {
            Player.Queue.NextPositionRequest = position;
        }

        [PamelloCommand]
        public async Task PlayerQueueRandom(bool value) {
            Player.Queue.IsRandom = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueReversed(bool value) {
            Player.Queue.IsReversed = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueNoLeftovers(bool value) {
            Player.Queue.IsNoLeftovers = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueFeedRandom(bool value) {
            Player.Queue.IsFeedRandom = value;
        }

        [PamelloCommand]
        public async Task PlayerQueueShuffle() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlayerQueueClear() {
            Player.Queue.Clear();
        }

        //song
        [PamelloCommand]
        public async Task<PamelloSong> SongAdd(string youtubeId) {
            return await _songs.AddAsync(youtubeId, User) ?? throw new PamelloException($"Cant add youtube song with id \"{youtubeId}\"");
        }
        [PamelloCommand]
        public async Task<string> SongRename(PamelloSong song, string newName) {
            return song.Name = newName;
        }

        [PamelloCommand]
        public async Task SongFavoriteAdd(PamelloSong song) {
            User.AddFavoriteSong(song);
        }
        [PamelloCommand]
        public async Task SongFavoriteRemove(PamelloSong song) {
            User.RemoveFavoriteSong(song);
        }

        [PamelloCommand]
        public async Task SongAssociationsAdd(PamelloSong song, string associacion) {
            song.AddAssociation(associacion);
        }
        [PamelloCommand]
        public async Task SongAssociationsRemove(PamelloSong song, string associacion) {
            song.RemoveAssociation(associacion);
        }

        [PamelloCommand]
        public async Task<PamelloEpisode> SongEpisodeAdd(PamelloSong song, int episodeStart, string episodeName) {
            return song.AddEpisode(new AudioTime(episodeStart), episodeName, false);
        }
        [PamelloCommand]
        public async Task SongEpisodeRemove(PamelloSong song, int episodePosition) {
            song.RemoveEpisodeAt(episodePosition);
        }
        [PamelloCommand]
        public async Task<string> SongEpisodeRename(PamelloSong song, int episodePosition, string newName) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            return episode.Name = newName;
        }
        [PamelloCommand]
        public async Task SongEpisodeSkipSet(PamelloSong song, int episodePosition, bool newState) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.AutoSkip = newState;
        }
        [PamelloCommand]
        public async Task<int> SongEpisodeEditTime(PamelloSong song, int episodePosition, int newTime) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Start = new AudioTime(newTime);
            return episode.Start.TotalSeconds;
        }
        [PamelloCommand]
        public async Task SongEpisodesClear(PamelloSong song) {
            _episodes.DeleteAllFrom(song);
        }
    
        //playlist
        [PamelloCommand]
        public async Task<PamelloPlaylist> PlaylistCreate(string name, bool fillWithQueue) {
            var playlist = User.CreatePlaylist(name);

            if (fillWithQueue) {
                playlist.AddList(Player.Queue.Songs);
            }

            return playlist;
        }
        [PamelloCommand]
        public async Task<PamelloSong?> PlaylistAddSong(PamelloPlaylist playlist, PamelloSong song, int? position = null) {
            return playlist.AddSong(song, position);
        }
        [PamelloCommand]
        public async Task PlaylistAddPlaylistSongs(PamelloPlaylist fromPlaylist, PamelloPlaylist toPlaylist, int? position = null) {
            toPlaylist.AddList(fromPlaylist.Songs, position);
        }
        [PamelloCommand]
        public async Task<int> PlaylistRemoveSong(PamelloPlaylist playlist, PamelloSong song) {
            return playlist.RemoveSong(song);
        }
        [PamelloCommand]
        public async Task<PamelloSong?> PlaylistRemoveAt(PamelloPlaylist playlist, int position) {
            return playlist.RemoveAt(position);
        }
        [PamelloCommand]
        public async Task<PamelloSong?> PlaylistMoveSong(PamelloPlaylist playlist, int fromPosition, int toPosition) {
            return playlist.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public async Task<string> PlaylistRename(PamelloPlaylist playlist, string newName) {
            return playlist.Name = newName;
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteAdd(PamelloPlaylist playlist) {
            User.AddFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteRemove(PamelloPlaylist playlist) {
            User.RemoveFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistDelete(PamelloPlaylist playlist) {
            _playlists.Delete(playlist);
        }
        
        //speakers
        [PamelloCommand]
        public async Task<PamelloDiscordSpeaker> SpeakerDiscordConnect() {
            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) throw new PamelloException("You have to be in voce channel to execute this command");

            return await _speakers.ConnectDiscord(Player, vc.Guild.Id, vc.Id);
        }
        [PamelloCommand]
        public async Task<PamelloInternetSpeaker> SpeakerInternetConnect(string? name) {
            var speaker = await _speakers.ConnectInternet(Player, name);

            return speaker;
        }
        [PamelloCommand]
        public async Task SpeakerDisconnect(PamelloSpeaker speaker)
        {
            await _speakers.Delete(speaker);
        }
        [PamelloCommand]
        public async Task<string> SpeakerInternetRename(PamelloInternetSpeaker speaker, string newName) {
            return speaker.Name = newName;
        }
    }
}
