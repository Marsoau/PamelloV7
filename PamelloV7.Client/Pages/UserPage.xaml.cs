using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Windows;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
using System.Windows.Controls;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        private readonly MainWindow _mainWindow;

        private RemoteUser? _user;
        public RemoteUser? User {
            get => _user;
            set {
                _user = value;

                Refresh();
            }
        }

        public UserPage(IServiceProvider services) {
            _services = services;

            _mainWindow = services.GetRequiredService<MainWindow>();

            _pamello = services.GetRequiredService<PamelloClient>();

            InitializeComponent();

            SubscribeToEvents();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            Refresh();
        }

        private void Button_Back_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<MainPage>();
        }
    }
}
