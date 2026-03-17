using System.Diagnostics;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.Audio.Modules.Base;

public abstract class AudioModule : IAudioModule
{
    public List<AudioPoint> Inputs { get; protected set; } = null!;
    public virtual int MinInputs => 0;
    public virtual int MaxInputs => 0;
    public List<AudioPoint> Outputs { get; protected set; } = null!;
    public virtual int MinOutputs => 0;
    public virtual int MaxOutputs => 0;
    
    public void InitAudio(IServiceProvider services) {
        Debug.Assert(Inputs is not null);
        Debug.Assert(Outputs is not null);
        
        InitAudioInternal(services);
    }

    public AudioPoint AddInput() {
        if (Inputs.Count >= MaxInputs) throw new PointsLimitException();
        
        var input = new AudioPoint(this);
        
        AddInputInternal(input);
        
        return input;
    }
    public AudioPoint AddOutput() {
        if (Outputs.Count >= MaxOutputs) throw new PointsLimitException();
        
        var input = new AudioPoint(this);
        
        AddOutputInternal(input);
        
        return input;
    }

    public void RemoveInput() => RemoveInput(Inputs.LastOrDefault() ?? throw new PointsLimitException());
    public void RemoveInput(AudioPoint point) => RemoveInput(Inputs.IndexOf(point));
    public void RemoveInput(int index) {
        if (Inputs.Count <= MinInputs) throw new PointsLimitException();
        
        RemoveInputInternal(index);
    }
    
    public void RemoveOutput() => RemoveOutput(Outputs.LastOrDefault() ?? throw new PointsLimitException());
    public void RemoveOutput(AudioPoint point) => RemoveOutput(Outputs.IndexOf(point));
    public void RemoveOutput(int index) {
        if (Outputs.Count <= MinOutputs) throw new PointsLimitException();
        
        RemoveOutputInternal(index);
    }
    
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this); // Fixes CA1816
    }

    protected virtual void Dispose(bool isDisposing) {
        foreach (var input in Inputs) {
            input.ProcessAudio = null;
            input.ConnectedPoint = null;
        }

        foreach (var output in Outputs) {
            output.ProcessAudio = null;
            output.ConnectedPoint = null;
        }
    }

    protected virtual void AddInputInternal(AudioPoint point) {
        Inputs.Add(point);
    }
    protected virtual void AddOutputInternal(AudioPoint point) {
        Outputs.Add(point);
    }
    protected virtual void RemoveInputInternal(int index) {
        Inputs.RemoveAt(index);
    }
    protected virtual void RemoveOutputInternal(int index) {
        Outputs.RemoveAt(index);
    }

    protected virtual void InitAudioInternal(IServiceProvider services) { }
}
