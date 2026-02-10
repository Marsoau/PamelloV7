using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Repositories.Base;

namespace PamelloV7.Module.Marsoau.Base.Repositories;

public class PamelloSpeakerRepository : PamelloRepository<IPamelloSpeaker>, IPamelloSpeakerRepository
{
    private readonly IPamelloAudioSystem _audio;
    
    public int NextId { get; private set; }
    
    public PamelloSpeakerRepository(IServiceProvider services) : base(services) {
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        NextId = 1;
    }
    
    private static IPamelloSpeaker? ReturnIfAvailable(IPamelloSpeaker? speaker, IPamelloUser scopeUser) {
        if (!speaker?.IsAvailableFor(scopeUser) ?? false) return null;
        return speaker;
    }

    public IPamelloSpeaker? Get(IPamelloUser scopeUser, int id) {
        return ReturnIfAvailable(Get(id), scopeUser);
    }

    public IPamelloSpeaker? GetByName(IPamelloUser scopeUser, string query) {
        var speaker = _loaded.FirstOrDefault(speaker => speaker.Name == query);
        return ReturnIfAvailable(speaker, scopeUser);
    }

    public IEnumerable<IPamelloSpeaker> GetCurrent(IPamelloUser scopeUser) {
        return scopeUser.SelectedPlayer?.ConnectedSpeakers.Where(speaker => speaker.IsAvailableFor(scopeUser)) ?? [];
    }

    public IEnumerable<IPamelloSpeaker> GetRandom(IPamelloUser scopeUser) {
        var all = GetAll(scopeUser).ToArray();
        var speaker = all.ElementAtOrDefault(Random.Shared.Next(all.Length));
        return speaker is not null ? [speaker] : [];
    }

    public IEnumerable<IPamelloSpeaker> GetFromPlayer(IPamelloUser scopeUser, IPamelloPlayer player) {
        return player.ConnectedSpeakers.Where(speaker => speaker.IsAvailableFor(scopeUser));
    }

    public IEnumerable<IPamelloSpeaker> GetAll(IPamelloUser scopeUser) {
        return _loaded.Where(speaker => speaker.IsAvailableFor(scopeUser));
    }

    public TPamelloSpeaker Add<TPamelloSpeaker>(TPamelloSpeaker speaker)
        where TPamelloSpeaker : IPamelloSpeaker
    {
        if (speaker.Id != NextId) throw new Exception($"Speaker id is out of repository order, was {speaker.Id} instead of {NextId}");
        NextId++;

        if (speaker is IAudioDependant dependant)
            _audio.RegisterDependant(dependant);
        
        _loaded.Add(speaker);
        
        return speaker;
    }

    public override void Delete(IPamelloSpeaker entity) {
        throw new NotImplementedException();
    }
}
