using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.User;

public class UserAuthorizationSelectModal : DiscordModal
{
    public static Modal Build(IPamelloUser user, IServiceProvider services) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var count = 0;
        
        var modalBuilder = new ModalBuilder()
            .WithTitle("Select authorization")
            .WithCustomId($"authorization-select-modal:{user.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithSelectMenu("Authorization", new SelectMenuBuilder()
                    .WithCustomId("modal-select")
                    .WithOptions(user.Authorizations.Select(authorization => new SelectMenuOptionBuilder()
                        .WithValue(count.ToString())
                        .WithLabel(authorization.PK.Key)
                        .WithEmote(clients.GetEmote(authorization.PK.Platform).Result)
                        .WithDefault(count++ == user.SelectedAuthorizationIndex)
                    ).ToList())
                    .WithRequired(true)
                )
            );
        
        return modalBuilder.Build();
    }
    
    [ModalSubmission("authorization-select-modal")]
    public async Task Submit(string userQuery) {
        var song = _peql.GetSingleRequired<IPamelloSong>(userQuery, User);
        var authorizationString = GetSelectValue("modal-select");
        Console.WriteLine($"authorizationString: {authorizationString}");
        if (!int.TryParse(authorizationString, out var authorizationIndex)) throw new PamelloException("Invalid authorization index key");

        Command<UserAuthorizationSelect>().Execute(authorizationIndex);
        
        await ReleaseInteractionAsync();
    }
}
