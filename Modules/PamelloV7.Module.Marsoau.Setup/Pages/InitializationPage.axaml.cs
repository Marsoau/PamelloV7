using Avalonia.Controls;
using Avalonia.Interactivity;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Module.Marsoau.Setup.Controls;
using PamelloV7.Module.Marsoau.Setup.Pages.Base;

namespace PamelloV7.Module.Marsoau.Setup.Pages;

public partial class InitializationPage : SetupPage
{
    public InitializationPage() {
        InitializeComponent();
    }

    public void AddPreInitializers(IEnumerable<PamelloConfigPreInitializer> preInitializers) {
        foreach (var preInitializer in preInitializers) {
            Stack.Children.Add(new PreInitializerControl(preInitializer));
        }
    }

    public void ApplyPreInitializers() {
        foreach (var preInitializer in Stack.Children.OfType<PreInitializerControl>()) {
            preInitializer.Apply?.Invoke();
        }
        
        CompletePage();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e) {
        CompletePage(true);
    }

    private void ApplyButton_OnClick(object? sender, RoutedEventArgs e) {
        ApplyPreInitializers();
    }
}
