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
        private readonly MainWindow _mainWindow;
        private readonly PamelloClient _pamello;

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
        }

        public async Task Update()
            => await Update(null);
        public async Task Update(int? userId) {
            var id = userId ?? _user?.Id;

            if (id is not null) _user = await _pamello.Users.GetNew(id.Value, true);

            Refresh();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            Refresh();
        }

        private void Button_Back_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<MainPage>();
        }
    }
}
