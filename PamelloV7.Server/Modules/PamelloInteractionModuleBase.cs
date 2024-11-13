using Discord;
using Discord.Interactions;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Modules
{
    public class PamelloInteractionModuleBase : InteractionModuleBase<PamelloSocketInteractionContext>
    {
        private readonly UserAuthorizationService _authorization;

        public PamelloInteractionModuleBase(IServiceProvider services) {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
        }

        protected async Task ModifyWithEmbedAsync(Embed embed) {
                await ModifyOriginalResponseAsync(message => message.Embed = embed);
        }

        //general
        public async Task Ping() {
            await ModifyWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Pong!", ""));
        }

        public async Task GetCode() {
            await ModifyWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Authorization Code", _authorization.GetCode(Context.User.DiscordUser.Id).ToString()));
        }
    }
}
