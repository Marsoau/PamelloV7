using osu.NET;
using osu.NET.Authorization;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Osu.Config;

namespace PamelloV7.Module.Marsoau.Osu.Services;

public class OsuClientService : IPamelloService
{
    public readonly OsuApiClient Client;

    public OsuClientService(IServiceProvider services) {
        Client = new OsuApiClient(
            new OsuClientAccessTokenProvider(
                OsuConfig.Root.Authorization.ApplicationId,
                OsuConfig.Root.Authorization.Token
            )
        , null);
    }
}
