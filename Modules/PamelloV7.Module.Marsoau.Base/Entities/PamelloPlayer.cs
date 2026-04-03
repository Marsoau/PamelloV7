using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Module.Marsoau.Base.Queue;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloPlayer : PamelloDynamicEntity, IPamelloPlayer, IAudioDependant
{
    private readonly IPamelloAudioSystem _audio;
    
    public IPamelloUser Owner { get; }
    
    private string _name;

    public override string Name => _name;
    public override string SetName(string value, IPamelloUser scopeUser) {
        if (_name == value) return _name;

        _name = value;
        _sink.Invoke(scopeUser, new PlayerNameUpdated() {
            Player = this,
            Name = _name
        });
        
        return _name;
    }

    public override bool IsDeleted { get; set; }

    public EPlayerState State { get; private set; }
    public bool IsProtected { get; set; }

    [OnAudioMap]
    public bool IsPaused { get; private set; }
    public bool SetPause(bool state, IPamelloUser? scopeUser) {
        if (IsPaused == state) return IsPaused;

        IsPaused = state;
        
        _sink.Invoke(scopeUser, new PlayerIsPausedUpdated() {
            Player = this,
            IsPaused = IsPaused,
        });
        
        return IsPaused;
    }
    
    private List<IPamelloSpeaker> _connectedSpeakers;

    public IPamelloQueue? Queue { get; }
    public IPamelloQueue RequiredQueue => Queue ?? throw new PamelloException("Player doesnt have a queue");
    public IEnumerable<IPamelloSpeaker> ConnectedSpeakers => _connectedSpeakers;

    [OnAudioMap]
    public AudioPump Pump { get; set; }
    IAudioModule IPamelloPlayer.Pump => Pump;
    
    [OnAudioMap]
    public AudioCopy Copy { get; set; }
    IAudioModule IPamelloPlayer.Copy => Copy;

    private static int _idCounter = 1;
    public PamelloPlayer(string name, IPamelloUser owner, IServiceProvider services) : base(_idCounter, services) {
        Interlocked.Increment(ref _idCounter);
        
        Owner = owner;
        
        _name = name;
        
        _connectedSpeakers = [];
        
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Queue = _audio.RegisterDependant(new PamelloQueue(this, services));

        Pump = _audio.RegisterModule(new AudioPump(40960));
        Copy = _audio.RegisterModule(new AudioCopy());
    }

    public void InitDependant() {
        Pump.Output.ConnectedPoint = Copy.Input;
        
        Pump.Condition = () => !IsPaused;
        
        Pump.Start();
    }

    public bool IsAvailableFor(IPamelloUser user) {
        return user == Owner || _connectedSpeakers.Any(speaker => speaker.IsAvailableFor(user));
    }
    
    public IPamelloSpeaker AddSpeaker(IPamelloSpeaker speaker) {
        Copy.AddOutput().ConnectedPoint = speaker.InputModule.Input;
        
        _connectedSpeakers.Add(speaker);

        return speaker;
    }

    public override PamelloEntityDto GetDto() {
        return new PamelloPlayerDto {
            Id = Id,
            Name = Name,
            Owner = Owner.Id,
            IsProtected = IsProtected,
            //State = State,
            IsPaused = IsPaused,
            Queue = Queue?.GetDto(),
        };
    }
}
