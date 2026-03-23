using Avalonia.Controls;

namespace PamelloV7.Framework.Consolonia.Screens;

public class PamelloScreen : UserControl
{
    public readonly TaskCompletionSource LoadingCompleted = new();
    
    public PamelloScreen() {
        Loaded += (_, _) => LoadingCompleted.SetResult();
    }
}
