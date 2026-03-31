using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
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
        var groups = preInitializers.GroupBy(preInitializer => preInitializer.Part.Module);
        foreach (var group in groups) {
            var separatorGrid = new Grid() {
                Margin = new Thickness(0, 1, 0, 0),
                ColumnDefinitions = new ColumnDefinitions("*, Auto, *"),
            };

            var leftSeparator = new Separator();
            var rightSeparator = new Separator();

            var label = new TextBlock() {
                Text = group.Key is null ? "Server" : $"{group.Key.Author}/{group.Key.Name}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = group.Key?.Color ?? Brushes.MediumPurple,
            };
            
            separatorGrid.Children.Add(leftSeparator);
            separatorGrid.Children.Add(label);
            separatorGrid.Children.Add(rightSeparator);
            
            Grid.SetColumn(leftSeparator, 0);
            Grid.SetColumn(label, 1);
            Grid.SetColumn(rightSeparator, 2);
            
            Stack.Children.Add(separatorGrid);
            
            foreach (var preInitializer in group) {
                Stack.Children.Add(new PreInitializerControl(preInitializer));
            }
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
