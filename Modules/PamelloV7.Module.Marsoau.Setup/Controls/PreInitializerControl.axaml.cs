using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using PamelloV7.Framework.Config.Parts;

namespace PamelloV7.Module.Marsoau.Setup.Controls;

public partial class PreInitializerControl : UserControl
{
    public readonly PamelloConfigPreInitializer PreInitializer;
    
    public string Path => PreInitializer.PropertyPath;
    public string Type => PreInitializer.PropertyType.Name;
    
    public Action? Apply { get; private set; }

    public PreInitializerControl() {
        throw new Exception("This constructor is not intended to be used");
    }
    public PreInitializerControl(PamelloConfigPreInitializer preInitializer) {
        PreInitializer = preInitializer;
        
        InitializeComponent();
        
        DataContext = this;
        
        AddInput();
    }

    public void AddInput() {
        Control? input = null;
        
        if (PreInitializer.PropertyType == typeof(bool)) {
            var check = new CheckBox() {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = Brushes.Transparent,
            };
            input = check;

            void SetCheckContent() {
                check.Content = check.IsChecked ?? false ? "True" : "False";
            }

            SetCheckContent();
            check.IsCheckedChanged += (_, _) => SetCheckContent();
            
            Apply = () => PreInitializer.Value = check.IsChecked;
        }
        else if (PreInitializer.PropertyType.IsEnum) {
            var values = Enum.GetValues(PreInitializer.PropertyType);
            var combo = new ComboBox();
            
            combo.ItemsSource = values;
            combo.SelectedIndex = 0;
            
            input = combo;
            
            Apply = () => PreInitializer.Value = values.GetValue(combo.SelectedIndex);
        }
        else {
            var text = new TextBox();
            input = text;
            
            var converter = TypeDescriptor.GetConverter(PreInitializer.PropertyType);
            
            Apply = () => PreInitializer.Value = converter.ConvertFromString(text.Text ?? "");
        }
        
        Grid.Children.Add(input);
        Grid.SetColumn(input, 2);
    }
}
