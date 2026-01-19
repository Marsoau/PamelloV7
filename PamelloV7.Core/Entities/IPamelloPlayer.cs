using PamelloV7.Core.Audio;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Entities;

public interface IPamelloPlayer : IPamelloEntity
{
    public IPamelloUser Creator { get; }
    
    public EPlayerState State { get; set; }
    public bool IsProtected { get; set; }
    public bool IsPaused { get; set; }
    
    public IPamelloQueue Queue { get; }

    public Task<IPamelloInternetSpeaker> AddInternet(string? name);
    //other speaker providers should probably create extension methods here
}
