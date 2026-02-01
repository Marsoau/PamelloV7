using PamelloV7.Audio.Points;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;

namespace PamelloV7.Audio.Services;

public class PamelloAudioSystem : IPamelloAudioSystem
{
    public List<IAudioModule> Modules { get; }
    
    public PamelloAudioSystem() {
        Modules = [];
    }
    
    public TAudioModule Register<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule
    {
        Modules.Add(module);

        if (module is IAudioModuleWithInputs moduleWithModel) {
            for (var i = 0; i < moduleWithModel.MinInputs; i++) {
                moduleWithModel.AddInput(() => new AudioPoint(module));
            }
        }

        if (module is IAudioModuleWithOutputs moduleWithOutputs) {
            for (var i = 0; i < moduleWithOutputs.MinOutputs; i++) {
                moduleWithOutputs.AddOutput(() => new AudioPoint(module));
            }
        }
        
        module.InitAudio();
        
        return module;
    }
}
