using Avalonia.Controls;
using Avalonia.Threading;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Module.Marsoau.Setup.Steps.Base;

public abstract partial class SetupStep : UserControl
{
    public TaskCompletionSource<bool> StepCompleted { get; } = new();
    
    public IPamelloModule? Module { get; private set; }
    
    public abstract string Description { get; }

    protected void AddControlsInternal() {
        Dispatcher.UIThread.Invoke(AddControls);
    }
    public abstract void AddControls();
    
    public SetupStep() {
        InitializeComponent();
        
        DataContext = this;
        
        AddControlsInternal();
    }
    
    public void CompleteStep(bool abortModuleSetup = false) => StepCompleted.SetResult(abortModuleSetup);

    public void AddInput(string name) {
        
    }
}
