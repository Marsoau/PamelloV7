using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;
using System;
using System.Collections.Generic;

namespace PamelloV7.Server.Model.Audio;

public class AudioModel : IDisposable
{
    public readonly List<IAudioModule> Modules;

    public AudioModel() {
        Modules = [];

        _ = GarbageCollecting();
    }

    private async Task GarbageCollecting()
    {
        while (true)
        {
            await Task.Delay(3000);
            ClearGarbage();
        }
    }
    
    public TModule AddModule<TModule>(TModule module)
        where TModule : IAudioModule
    {
        if (module == null) {
            throw new ArgumentNullException(nameof(module), "Cannot add null module to AudioModel");
        }
        
        Modules.Add(module);
        
        Console.WriteLine($"Adding module of type {module.GetType().Name} to audio model");
    
        if (module is IAudioModuleWithModel moduleWithModel) {
            Console.WriteLine($"Initializing model for {module.GetType().Name}");
            moduleWithModel.InitModel();
        }
        
        // Handle IAudioModuleWithInputs<T> regardless of T
        try {
            InitInputsForModule(module);
        }
        catch (Exception ex) {
            Console.WriteLine($"Error initializing inputs for module {module.GetType().Name}: {ex.Message}");
        }
        
        // Handle IAudioModuleWithOutputs<T> regardless of T
        try {
            InitOutputsForModule(module);
        }
        catch (Exception ex) {
            Console.WriteLine($"Error initializing outputs for module {module.GetType().Name}: {ex.Message}");
        }
        
        Console.WriteLine($"Calling InitModule for {module.GetType().Name}");
        module.InitModule();

        return module;
    }

    public void AddModules(IEnumerable<IAudioModule> modules) {
        foreach (var module in modules) {
            AddModule(module);
        }
    }

    private void InitInputsForModule(IAudioModule module)
    {
        // Handle AudioPushPoint type
        if (module is IAudioModuleWithInputs<AudioPushPoint> pushInputModule) {
            for (var i = 0; i < pushInputModule.MinInputs; i++) {
                pushInputModule.CreateInput();
            }
            return;
        }
        
        // Handle AudioPullPoint type
        if (module is IAudioModuleWithInputs<AudioPullPoint> pullInputModule) {
            for (var i = 0; i < pullInputModule.MinInputs; i++) {
                pullInputModule.CreateInput();
            }
            return;
        }
        
        // Fallback for other IAudioPoint implementations if needed
        if (module is IAudioModuleWithInputs<IAudioPoint> genericInputModule) {
            for (var i = 0; i < genericInputModule.MinInputs; i++) {
                genericInputModule.CreateInput();
            }
        }
    }
    
    private void InitOutputsForModule(IAudioModule module)
    {
        // Handle AudioPushPoint type
        if (module is IAudioModuleWithOutputs<AudioPushPoint> pushOutputModule) {
            for (var i = 0; i < pushOutputModule.MinOutputs; i++) {
                pushOutputModule.CreateOutput();
            }
            return;
        }
        
        // Handle AudioPullPoint type
        if (module is IAudioModuleWithOutputs<AudioPullPoint> pullOutputModule) {
            for (var i = 0; i < pullOutputModule.MinOutputs; i++) {
                pullOutputModule.CreateOutput();
            }
            return;
        }
        
        // Fallback for other IAudioPoint implementations if needed
        if (module is IAudioModuleWithOutputs<IAudioPoint> genericOutputModule) {
            for (var i = 0; i < genericOutputModule.MinOutputs; i++) {
                genericOutputModule.CreateOutput();
            }
        }
    }

    public void ClearGarbage()
    {
        var disposed = Modules.Where(module => module.IsDisposed).ToList();
        
        foreach (var module in disposed) {
            Console.WriteLine("removing disposed module");
            Modules.Remove(module);
        }
    }

    public void Dispose()
    {
        foreach (var module in Modules) {
            module.Dispose();
        }
        ClearGarbage();
    }
}