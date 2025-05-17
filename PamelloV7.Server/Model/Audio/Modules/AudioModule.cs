using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Audio.Modules;

public abstract class AudioModule<TInput, TOutput>
    where TInput : AudioPoint, new()
    where TOutput : AudioPoint, new()
{
    protected abstract int MinInputs { get; }
    protected abstract int MaxInputs { get; }
    
    protected abstract int MinOutputs { get; }
    protected abstract int MaxOutputs { get; }

    public readonly List<TInput> Inputs;
    public readonly List<TOutput> Outputs;

    protected AudioModule() {
        Inputs = [];
        Outputs = [];
    }

    public virtual TInput CreateInput() {
        var input = new TInput();
        
        Inputs.Add(input);
        
        return input;
    }

    public virtual TOutput CreateOutput() {
        var output = new TOutput();
        
        Outputs.Add(output);
        
        return output;
    }

    public virtual void Init() {
        for (int i = 0; i < MinInputs; i++) {
            CreateInput();
        }
        
        for (int i = 0; i < MinOutputs; i++) {
            CreateOutput();
        }
    }
}