using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.User;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.User.Authorization;

[DiscordCommand("user authorization list", "View/Manage your authorizations")]
public partial class UserAuthorizationList
{
    public async Task Execute(
        [UserDescription] IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        await RespondPageAsync(page => 
                Builder<Builder>().Build(user, page)
            , () => [ScopeUser]);
    }
    
    public class Builder : DiscordComponentBuilder
    {
        private enum AuthorizationListMode
        {
            Select,
            Delete
        }
        
        private AuthorizationListMode _mode;
        
        private DiscordClientService? _clients;
        public IMessageComponentProperties?[] Build(IPamelloUser user, int page) {
            _clients ??= Services.GetRequiredService<DiscordClientService>();
            
            var container = new ComponentContainerProperties();

            var isOwner = user == ScopeUser;
            
            const int pageSize = 5;
            
            var totalPages = user.Authorizations.Count / pageSize + (user.Authorizations.Count % pageSize > 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;

            var itemsOnPage = user.Authorizations.Skip(page * pageSize).Take(pageSize).ToList();
            
            container.AddComponents(
                new TextDisplayProperties($"## Authorizations{(
                    isOwner ? "" : $" of {user.ToDiscordString()}"
                )}"),
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    ModalButton<UserAuthorizationAddModal>("Add Authorization", ButtonStyle.Primary)
                ),
                new ComponentSeparatorProperties()
            );
            
            if (itemsOnPage.Count > 0) {
                var count = page * pageSize;
                foreach (var authorization in itemsOnPage) {
                    ButtonProperties button;

                    if (authorization == ScopeUser.SelectedAuthorization) {
                        button = Button(count++, "Selected", ButtonStyle.Secondary, () => { }).WithDisabled();
                    }
                    else if (
                        _mode == AuthorizationListMode.Delete &&
                        authorization.PK.Platform == "discord" &&
                        authorization.PK.Key == Message.Command.Interaction.User.Id.ToString()
                    ) {
                        button = Button(count++, "Current", ButtonStyle.Secondary, () => { }).WithDisabled();
                    }
                    else {
                        button = _mode switch {
                            AuthorizationListMode.Select =>
                                Button(count++, "Select", ButtonStyle.Secondary, () => {
                                    var index = user.Authorizations.ToList().IndexOf(authorization);
                                    Command<UserAuthorizationSelect>().Execute(index);
                                }).WithDisabled(!isOwner),
                            AuthorizationListMode.Delete =>
                                Button(count++, "Delete", ButtonStyle.Danger, () => {
                                    var index = user.Authorizations.ToList().IndexOf(authorization);
                                    Command<UserAuthorizationDelete>().Execute(index);
                                }).WithDisabled(!isOwner),
                            _ => throw new Exception()
                        };
                    }
                    
                    container.AddComponents(
                        new ComponentSectionProperties(button).AddComponents(
                            new TextDisplayProperties($"{DiscordString.Emoji(authorization.PK.Platform, _clients)} {DiscordString.Code(authorization.PK.Key)}")
                        )
                    );
                }
            }
            else {
                container.AddComponents(
                    new TextDisplayProperties($"-# {DiscordString.Italic("No episodes")}")
                );
            }

            if (isOwner) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new ActionRowProperties().AddComponents(
                        Button("Select", _mode == AuthorizationListMode.Select ? ButtonStyle.Primary : ButtonStyle.Secondary, async () => {
                            if (_mode == AuthorizationListMode.Select) return;
                        
                            _mode = AuthorizationListMode.Select;
                            await Message.Refresh();
                        }),
                        Button("Delete", _mode == AuthorizationListMode.Delete ? ButtonStyle.Primary : ButtonStyle.Secondary, async () => {
                            if (_mode == AuthorizationListMode.Delete) return;
                        
                            _mode = AuthorizationListMode.Delete;
                            await Message.Refresh();
                        })
                    )
                );
            }

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({user.Authorizations.Count} authorizations)")
            );

            return [
                container,
                Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, user.Authorizations.Count),
                Builder<BasicButtonsBuilder>().RefreshButtonRow()
            ];
        }
    }
}
