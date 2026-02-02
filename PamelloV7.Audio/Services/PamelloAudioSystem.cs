using PamelloV7.Audio.Points;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;
using PamelloV7.Core.Audio.Services;

namespace PamelloV7.Audio.Services;

public class PamelloAudioSystem : IPamelloAudioSystem
{
    private readonly List<IAudioModule> _modules;
    private readonly List<IAudioDependant> _dependants;
    
    public PamelloAudioSystem() {
        _modules = [];
        _dependants = [];
    }
    
    public TAudioModule RegisterModule<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule
    {
        Console.WriteLine($"Registering module: {module.GetType().FullName}");
        _modules.Add(module);

        if (module is IAudioModuleWithInputs moduleWithInputs) {
            for (var i = moduleWithInputs.Inputs.Count; i < moduleWithInputs.MinInputs; i++) {
                moduleWithInputs.AddInput(() => new AudioPoint(module));
            }
        }

        if (module is IAudioModuleWithOutputs moduleWithOutputs) {
            for (var i = moduleWithOutputs.Outputs.Count; i < moduleWithOutputs.MinOutputs; i++) {
                moduleWithOutputs.AddOutput(() => new AudioPoint(module));
            }
        }
        
        module.InitAudio();
        
        return module;
    }

    public void DeleteModule<TAudioModule>(TAudioModule module) where TAudioModule : class, IAudioModule {
        Console.WriteLine($"Deleting module: {module.GetType().FullName}");
        
        _modules.Remove(module);
        
        if (module is IAudioModuleWithInputs moduleWithInputs) {
            foreach (var input in moduleWithInputs.Inputs) {
                input.ProcessAudio = null;
                input.ConnectedPoint = null;
            }
        }

        if (module is IAudioModuleWithOutputs moduleWithOutputs) {
            foreach (var output in moduleWithOutputs.Outputs) {
                output.ProcessAudio = null;
                output.ConnectedPoint = null;
            }
        }
    }

    public TAudioDependant RegisterDependant<TAudioDependant>(TAudioDependant dependant) where TAudioDependant : class, IAudioDependant {
        Console.WriteLine($"Registering dependant: {dependant.GetType().FullName}");
        
        _dependants.Add(dependant);
        
        dependant.InitDependant();
        
        return dependant;
    }

    public void Shutdown() {
        foreach (var module in _modules) DeleteModule(module);
    }
}
