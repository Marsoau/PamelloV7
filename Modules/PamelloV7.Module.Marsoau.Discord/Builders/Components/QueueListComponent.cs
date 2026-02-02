using Discord;
using Discord.Rest;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders.Components;

public static class QueueListComponent
{
    public static ComponentBuilderV2 Get(IPamelloUser scopeUser, int page, int pageSize) {
        var container = new ContainerBuilder();

        var queue = scopeUser.RequiredSelectedPlayer.RequiredQueue;
        var entries = queue.Entries.ToList();
        
        var totalPages = entries.Count / pageSize + (entries.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;
        
        var entriesOnPage = entries.Skip(page * pageSize).Take(pageSize).ToList();
        
        var counter = page * pageSize + 1;
        
        var content = entriesOnPage.Count == 0 ? "-# _No songs_" :
            string.Join("\n", entriesOnPage.Select(entry => {
                return $"`{counter++}` : {entry.Song.ToDiscordString()}";
            }));
        
        var contentBefore = entriesOnPage.Count == 0 ? "-# _No songs_" :
            string.Join("\n", entriesOnPage.Take(queue.Position).Select(entry => {
                return $"`{counter++}` : {entry.Song.ToDiscordString()}";
            }));
        var contentAfter = entriesOnPage.Count == 0 ? "" :
            string.Join("\n", entriesOnPage.Skip(entriesOnPage.Count - queue.Position + 1).Select(entry => {
                return $"`{counter++}` : {entry.Song.ToDiscordString()}";
            }));
            //string.Join("\n", songsOnPage.Select(entry => $"`{counter++}` : {entry.Song.ToDiscordString()}\n_added by_ {DiscordString.User(entry.Adder)}"));

            
        container
            .WithTextDisplay(
                $"""
                real queu {queue.Entries.Count} : {queue.Position}
                {content}
                """
            )
            .WithSeparator()
            .WithTextDisplay(
            $"""
            ## Queue
            {contentBefore}
            """
        );
        if (entriesOnPage.Count > 0) {
            container
                .WithSeparator()
                .WithTextDisplay(
                    "asd"
                );
            
            if (contentAfter.Length > 0)
                container
                    .WithSeparator()
                    .WithTextDisplay(contentAfter);
        }

        return PamelloComponentBuilders.PageButtons(
            PamelloComponentBuilders.RefreshButton(
                new ComponentBuilderV2().WithContainer(container)
            )
        , page != 0, page < totalPages - 1);
    }
}
