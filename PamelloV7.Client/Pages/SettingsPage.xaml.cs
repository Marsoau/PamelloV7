using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Windows;
using PamelloV7.Wrapper;
using System.Windows.Controls;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        private readonly MainWindow _mainWindow;

        public SettingsPage(IServiceProvider services) {
            _services = services;

            _mainWindow = services.GetRequiredService<MainWindow>();

            _pamello = services.GetRequiredService<PamelloClient>();

            InitializeComponent();
        }

        private void Button_Back_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<MainPage>();
        }
    }
}
