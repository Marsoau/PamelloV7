using Avalonia.Controls;

namespace PamelloV7.Module.Marsoau.Setup.Pages.Base;

public abstract class SetupPage : UserControl
{
    public TaskCompletionSource<bool> Completed { get; } = new();
    public void CompletePage(bool abort = false) => Completed.SetResult(abort);
}
