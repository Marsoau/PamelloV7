using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Audio.Services;

namespace PamelloV7.Audio.Services;

public class PamelloAudioSystem : IPamelloAudioSystem
{
    private readonly IServiceProvider _services;
    
    private readonly List<IAudioModule> _modules;
    private readonly List<IAudioDependant> _dependants;
    
    public PamelloAudioSystem(IServiceProvider services) {
        _services = services;
        
        _modules = [];
        _dependants = [];
    }
    
    public TAudioModule RegisterModule<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule
    {
        Console.WriteLine($"Registering module: {module.GetType().FullName}");
        _modules.Add(module);

        if (module is AudioModule aModule) {
            var inputsProperty = aModule.GetType().GetProperty(nameof(AudioModule.Inputs))!;
            var outputsProperty = aModule.GetType().GetProperty(nameof(AudioModule.Outputs))!;
            
            if (inputsProperty.GetValue(aModule) is null)
                inputsProperty.SetValue(aModule, new List<AudioPoint>());
            if (outputsProperty.GetValue(aModule) is null)
                outputsProperty.SetValue(aModule, new List<AudioPoint>());
        }

        for (var i = module.Inputs.Count; i < module.MinInputs; i++) {
            module.AddInput();
        }
        for (var i = module.Outputs.Count; i < module.MinOutputs; i++) {
            module.AddOutput();
        }
        
        module.InitAudio(_services);
        
        return module;
    }

    public void DeleteModule<TAudioModule>(TAudioModule module) where TAudioModule : class, IAudioModule {
        Console.WriteLine($"Deleting module: {module.GetType().FullName}");
        
        _modules.Remove(module);
        
        
        module.Dispose();
    }

    public TAudioDependant RegisterDependant<TAudioDependant>(TAudioDependant dependant) where TAudioDependant : class, IAudioDependant {
        Console.WriteLine($"Registering dependant: {dependant.GetType().FullName}");
        
        _dependants.Add(dependant);
        
        dependant.InitDependant();
        
        return dependant;
    }

    public void Shutdown() {
        foreach (var module in _modules.ToList()) DeleteModule(module);
    }
}
