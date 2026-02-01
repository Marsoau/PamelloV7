using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Base;

namespace PamelloV7.Module.Marsoau.Base.Repositories;

public class PamelloPlayerRepository : PamelloRepository<IPamelloPlayer>, IPamelloPlayerRepository
{
    private readonly IPamelloAudioSystem _audio;
    
    public PamelloPlayerRepository(IServiceProvider services) : base(services) {
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
    }

    private static IPamelloPlayer? ReturnIfAvailable(IPamelloPlayer? player, IPamelloUser scopeUser) {
        if (!player?.IsAvailableFor(scopeUser) ?? false) return null;
        return player;
    }

    public IPamelloPlayer? Get(IPamelloUser scopeUser, int id) {
        return ReturnIfAvailable(Get(id), scopeUser);
    }

    public IPamelloPlayer? GetByName(IPamelloUser scopeUser, string query) {
        var player = _loaded.FirstOrDefault(player => player.Name == query);
        return ReturnIfAvailable(player, scopeUser);
    }

    public IEnumerable<IPamelloPlayer> GetCurrent(IPamelloUser scopeUser) {
        return scopeUser.SelectedPlayer is not null ? [scopeUser.SelectedPlayer] : [];
    }

    public IEnumerable<IPamelloPlayer> GetRandom(IPamelloUser scopeUser) {
        var all = GetAll(scopeUser).ToArray();
        var player = all.ElementAtOrDefault(Random.Shared.Next(all.Length));
        return player is not null ? [player] : [];
    }

    public IEnumerable<IPamelloPlayer> GetAll(IPamelloUser scopeUser, IPamelloUser? ownedBy = null) {
        return _loaded.Where(player => player.IsAvailableFor(scopeUser));
    }

    public IPamelloPlayer Create(string name, IPamelloUser creator) {
        var oldName = name;
        for (var i = 1; _loaded.Any(player => player.Name == name); i++) {
            name = $"{oldName}-{i}";
        }
        
        var player = _audio.RegisterDependant(new PamelloPlayer(name, creator, _services));
        _loaded.Add(player);
        
        return player;
    }

    public override void Delete(IPamelloPlayer entity) {
        throw new NotImplementedException();
    }
}
