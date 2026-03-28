using osu.NET;
using osu.NET.Authorization;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Osu.Config;

namespace PamelloV7.Module.Marsoau.Osu.Services;

public class OsuClientService : IPamelloService
{
    public OsuApiClient Client {
        private set; get => field ?? throw new InvalidOperationException("Client not initialized");
    }

    public void Startup(IServiceProvider services) {
        Client = new OsuApiClient(
            new OsuClientAccessTokenProvider(
                OsuConfig.Root.Authorization.ApplicationId,
                OsuConfig.Root.Authorization.Token
            )
        , null);
    }
}
