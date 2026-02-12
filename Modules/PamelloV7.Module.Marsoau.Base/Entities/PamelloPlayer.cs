using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Audio.Points;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.Base.Queue;

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
    public bool IsPaused {
        get; set {
            if (field == value) return;

            field = value;
            
            _sink.Invoke(new PlayerIsPausedUpdated() {
                IsPaused = IsPaused,
            });
        }
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

    private static int _idCounter = 1;
    public PamelloPlayer(string name, IPamelloUser owner, IServiceProvider services) : base(_idCounter, services) {
        Interlocked.Increment(ref _idCounter);
        
        Owner = owner;
        
        _name = name;
        
        _connectedSpeakers = [];
        
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Queue = _audio.RegisterDependant(new PamelloQueue(this, services));

        Pump = _audio.RegisterModule(new AudioPump(4096));
        Copy = _audio.RegisterModule(new AudioCopy());
    }

    public void InitDependant() {
        Pump.Output.ConnectedPoint = Copy.Input;
        
        Pump.Condition = () => !IsPaused;
        
        Pump.Start();
    }

    public bool IsAvailableFor(IPamelloUser user) {
        return true; //Owner == user;
    }
    
    public IPamelloSpeaker AddSpeaker(IPamelloSpeaker speaker) {
        if (Copy is not IAudioModuleWithOutputs copy) throw new Exception("Copy is not IAudioModuleWithOutputs");
        if (speaker.Output is not IAudioModuleWithInput output) throw new Exception("Speaker output is not IAudioModuleWithInput");
        
        var point = copy.AddOutput(() => new AudioPoint(copy));
        point.ConnectedPoint = output.Input;
        
        _connectedSpeakers.Add(speaker);

        return speaker;
    }
}
