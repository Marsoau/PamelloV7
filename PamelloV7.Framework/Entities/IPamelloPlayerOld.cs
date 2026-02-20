using PamelloV7.Framework.AudioOld;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.Entities;

public interface IPamelloPlayerOld : IPamelloEntity
{
    public IPamelloUser Creator { get; }
    
    public EPlayerState State { get; set; }
    public bool IsProtected { get; set; }
    public bool IsPaused { get; set; }
    
    public IPamelloQueue Queue { get; }

    public Task<IPamelloInternetSpeaker> AddInternet(string? name);
    //other speaker providers should probably create extension methods here
}
