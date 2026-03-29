using Avalonia.Controls;
using Avalonia.Threading;
using PamelloV7.Framework.Consolonia.Screens;

namespace PamelloV7.Module.Marsoau.Setup.Screens;

public partial class SetupScreen : PamelloScreen
{
    public TaskCompletionSource SetupCompleted { get; } = new();
    
    public SetupScreen() {
        InitializeComponent();
    }

    public void SetPage(Control control) {
        Dispatcher.UIThread.Invoke(() => {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(control);
        });
    }
}
