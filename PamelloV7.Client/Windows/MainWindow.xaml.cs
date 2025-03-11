using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Pages;
using PamelloV7.Wrapper;

namespace PamelloV7.Client.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        public MainWindow(IServiceProvider services) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            _pamello.Events.OnConnection += Events_OnConnection;
            _pamello.OnAuthorized += _pamello_OnAuthorized;

            InitializeComponent();
        }

        private async Task _pamello_OnAuthorized() {
            Dispatcher.Invoke(SwitchPage<MainPage>);
        }

        private async Task Events_OnConnection() {
            Dispatcher.Invoke(SwitchPage<AuthorizationPage>);
        }

        public void SwitchPage<TPage>() where TPage : Page {
            Content = _services.GetRequiredService<TPage>();
        }
    }
}
