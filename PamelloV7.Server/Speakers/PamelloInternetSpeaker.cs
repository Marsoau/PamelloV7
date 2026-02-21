using PamelloV7.Audio.Modules;
using PamelloV7.Audio.Points;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Server.Config;
using PamelloV7.Server.Model.AudioOld.Modules.Pipes;

namespace PamelloV7.Server.Speakers;

public class PamelloInternetSpeaker : PamelloDynamicEntity, IPamelloInternetSpeaker, IAudioDependant
{
    private readonly IPamelloAudioSystem _audio;
    
    public override string Name { get; protected set; }
    public override string SetName(string name, IPamelloUser scopeUser) {
        return Name = name;
    }

    public AudioBuffer Buffer { get; }
    public AudioSilence Silence { get; }
    public AudioChoice Choice { get; }
    public AudioPump Pump { get; }
    public FFmpegMp3Converter Converter { get; }
    public AudioCopy Copy { get; }
    
    public IPamelloPlayer Player { get; }

    private readonly List<PamelloInternetSpeakerListener> _listeners;
    public IEnumerable<IPamelloListener> Listeners => _listeners;
    IAudioModule IPamelloSpeaker.InputModule => Buffer;
    
    public bool IsAvailableFor(IPamelloUser user) => true;
    
    public string GetUrl() {
        return $"https://{ServerConfig.Root.HostName}/Audio/Out/{Name}";
    }

    public PamelloInternetSpeaker(int id, string name, IPamelloPlayer player, IServiceProvider services) : base(id, services) {
        Player = player;
        
        Name = name;

        _listeners = [];
        
        _audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Buffer = _audio.RegisterModule(new AudioBuffer(102400));
        Silence = _audio.RegisterModule(new AudioSilence());
        Choice = _audio.RegisterModule(new AudioChoice());
        Pump = _audio.RegisterModule(new AudioPump(128));
        Converter = _audio.RegisterModule(new FFmpegMp3Converter());
        Copy = _audio.RegisterModule(new AudioCopy());
    }

    public void InitDependant() {
        if (Choice is not IAudioModuleWithInputs choice) throw new Exception("Choice is not IAudioModuleWithOutputs");
            
        var bufferChoiceInput = choice.AddInput(() => new AudioPoint(choice));
        var silenceChoiceInput = choice.AddInput(() => new AudioPoint(choice));
        
        Buffer.Output.ConnectedPoint = bufferChoiceInput;
        Silence.Output.ConnectedPoint = silenceChoiceInput;
        
        Pump.Input.ConnectedPoint = Choice.Output;
        Pump.Output.ConnectedPoint = Converter.Input;
        
        Converter.Output.ConnectedPoint = Copy.Input;
        
        Pump.Start();
        
        Player.AddSpeaker(this);
    }

    public async Task<IPamelloListener> CreateListener(HttpResponse response, CancellationToken requestAbortedToken, IPamelloUser? user) {
        if (Copy is not IAudioModuleWithOutputs copy) throw new Exception("Copy is not IAudioModuleWithOutputs");
        
        var listener = _audio.RegisterDependant(
            new PamelloInternetSpeakerListener(response, requestAbortedToken, this, user, _services)
        );
        
        await listener.Sink.InitializeConnection();

        var point = copy.AddOutput(() => new AudioPoint(copy));
        point.ConnectedPoint = listener.Sink.Input;
        
        _listeners.Add(listener);
        
        return listener;
    }
}
