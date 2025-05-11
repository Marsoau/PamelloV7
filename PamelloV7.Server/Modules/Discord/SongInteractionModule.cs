using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("song", "Commands to interact with songs")]
    public class SongInteractionModule : PamelloInteractionModuleBase
    {
        public SongInteractionModule(IServiceProvider services) : base(services) { }

        [SlashCommand("add", "Add new song to the database")]
        public async Task SongAddHandler(
            [Summary("youtube-url", "Youtube url of the song")] string youtubeUrl
        ) => await SongAdd(youtubeUrl);

        [SlashCommand("search", "Search songs in the database")]
        public async Task SongSearchHandler(
            [Summary("querry", "Song name querry")] string querry = "",
            [Summary("page", "Results page")] int page = 1,
            [Summary("added-by", "Filter by songs that were added by specified user")] SocketUser? addedByDiscordUser = null
        ) => await SongSearch(querry, page, addedByDiscordUser);

        [SlashCommand("info", "Get info about song")]
        public async Task SongInfoHandler(
            [Summary("song", "Song id/associacion/name/youtube-url")] string songValue = "current"
        ) => await SongInfo(songValue);

        [SlashCommand("rename", "Rename song")]
        public async Task SongRenameHandler(
            [Summary("name", "New name for the song")] string newName,
            [Summary("song", "Song id/associacion/name/youtube-url")] string songValue = "current"
        ) => await SongRename(songValue, newName);

        [Group("favorite", "Commands to manage favorite songs")]
        public class SongFavoriteInteractionModule : PamelloInteractionModuleBase
        {
            public SongFavoriteInteractionModule(IServiceProvider services) : base(services) { }

            [SlashCommand("add", "Add song to the favorites")]
            public async Task SongFavoriteAddHandler(
                [Summary("song", "Song id/associacion/name/youtube-url")] string songValue = "current"
            ) => await SongFavoriteAdd(songValue);

            [SlashCommand("remove", "Remove song from the favorites")]
            public async Task SongFavoriteRemoveHandler(
                [Summary("song", "Song id/associacion/name/youtube-url")] string songValue = "current"
            ) => await SongFavoriteRemove(songValue);

            [SlashCommand("list", "Show all your favorite songs")]
            public async Task SongFavoriteListHandler(
                [Summary("querry", "Song name querry")] string querry = "",
                [Summary("page", "Results page")] int page = 1,
                [Summary("favorite-by", "Get favorite songs of other user")] SocketUser? targetDiscordUser = null
            ) => await SongFavoriteList(querry, page, targetDiscordUser);
        }

        [Group("associations", "Commands to manage song associacions")]
        public class SongAssociationsInteractionModule : PamelloInteractionModuleBase
        {
            public SongAssociationsInteractionModule(IServiceProvider services) : base(services) { }


            [SlashCommand("add", "Add association to the song")]
            public async Task SongAssociationsAddHandler(
                [Summary("association", "New association name")] string association,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongAssociationsAdd(songValue, association);

            [SlashCommand("remove", "Remove association from the song")]
            public async Task SongAssociationsRemoveHandler(
                [Summary("association", "Association name")] string association,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongAssociationsRemove(songValue, association);

            [SlashCommand("list", "Show all song associations")]
            public async Task SongAssociationsListHandler(
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current",
                [Summary("page", "Results page")] int page = 1
            ) => await SongAssociationsList(songValue, page);
        }

        [Group("episodes", "Commands to manage song episodes")]
        public class SongEpisodesInteractionModule : PamelloInteractionModuleBase
        {
            public SongEpisodesInteractionModule(IServiceProvider services) : base(services) { }

            [SlashCommand("add", "Add episode to the song")]
            public async Task SongEpisodesAddHandler(
                [Summary("start", "Episode start time (format HH:MM:SS)")] string episodeTime,
                [Summary("name", "Episode name")] string episodeName,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesAdd(songValue, episodeTime, episodeName);

            [SlashCommand("rename", "Rename episode")]
            public async Task SongEpisodesRenameHandler(
                [Summary("new-name", "Episode new name")] string newName,
                [Summary("episode-position", "Episode position in song")] int episodePosition,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesRename(songValue, episodePosition, newName);

            [SlashCommand("skip", "Change episode skip state")]
            public async Task SongEpisodesChangeStartHandler(
                [Summary("skip", "New episode skip state")] EBoolState state,
                [Summary("episode-position", "Episode position in song")] int episodePosition,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesSkipSet(songValue, episodePosition, state);

            [SlashCommand("change-start", "Change episode start time")]
            public async Task SongEpisodesChangeStartHandler(
                [Summary("new-start", "New episode start time (format HH:MM:SS)")] string newTime,
                [Summary("episode-position", "Episode position in song")] int episodePosition,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesChangeStart(songValue, episodePosition, newTime);

            [SlashCommand("list", "Show episodes of the song")]
            public async Task SongEpisodesListHandler(
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current",
                [Summary("page", "Results page")] int page = 1
            ) => await SongEpisodesList(songValue, page);

            [SlashCommand("remove", "Remove episode from the song")]
            public async Task SongEpisodesRemoveHandler(
                [Summary("episode-position", "Episode position in song")] int episodePosition,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesRemove(songValue, episodePosition);

            [SlashCommand("clear", "Clear all episodes from the song")]
            public async Task SongEpisodesClearHandler(
                [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
            ) => await SongEpisodesClear(songValue);
        }
    }
}
