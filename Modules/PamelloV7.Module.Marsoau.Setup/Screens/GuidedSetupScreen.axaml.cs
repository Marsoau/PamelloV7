using PamelloV7.Framework.Consolonia.Screens;

namespace PamelloV7.Module.Marsoau.Setup.Screens;

public partial class GuidedSetupScreen : PamelloScreen
{
    public TaskCompletionSource SetupCompleted { get; } = new();
    
    public GuidedSetupScreen() {
        InitializeComponent();

        Loaded += async (_, _) => {
            await Task.Delay(5000);
            SetupCompleted.SetResult();
        };
    }
}
