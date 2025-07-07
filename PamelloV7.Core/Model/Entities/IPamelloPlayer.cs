using PamelloV7.Core.Audio;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.Audio;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloPlayer : IPamelloEntity
{
    public IPamelloUser Creator { get; }
    
    public EPlayerState State { get; set; }
    public bool IsProtected { get; set; }
    public bool IsPaused { get; set; }
    
    public IPamelloQueue Queue { get; }

    public Task<IPamelloInternetSpeaker> AddInternet(string? name);
}
