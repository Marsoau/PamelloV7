using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Module.Marsoau.Base.Queue;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloPlayer : PamelloEntity, IPamelloPlayer, IAudioDependant
{
    private readonly IPamelloAudioSystem _audio;
    
    public IPamelloUser Owner { get; }
    
    private string _name;
    
    public override string Name {
        get => _name;
        set {
            if (_name == value) return;
            
            _name = value;
            
            //invoke event
        }
    }

    public bool IsProtected { get; set; }
    
    [OnAudioMap]
    public bool IsPaused { get; set; }

    public IPamelloQueue? Queue { get; }

    [OnAudioMap]
    public AudioPump Pump { get; set; }
    IAudioModule IPamelloPlayer.Pump => Pump;
    
    [OnAudioMap]
    public AudioCopy Copy { get; set; }

    private static int _idCounter = 1;
    public PamelloPlayer(string name, IPamelloUser owner, IServiceProvider services) : base(_idCounter, services) {
        Interlocked.Increment(ref _idCounter);
        
        Owner = owner;
        
        _name = name;
        
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Queue = _audio.RegisterDependant(new PamelloQueue(this, services));

        Pump = _audio.RegisterModule(new AudioPump(4096));
        Copy = _audio.RegisterModule(new AudioCopy());
    }
    
    public bool IsAvailableFor(IPamelloUser user) {
        return Owner == user;
    }
}
