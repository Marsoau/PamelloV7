using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Enumerators;
using PamelloV7.Client.Model;
using PamelloV7.Client.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PamelloV7.Client.Components
{
    /// <summary>
    /// Interaction logic for ServerComponent.xaml
    /// </summary>
    public partial class ServerComponent : UserControl
    {
        private readonly IServiceProvider _services;

        public readonly SavedServer Server;

        private EServerComponentState _state;
        public EServerComponentState State {
            get => _state;
            set {
                switch (value) {
                    case EServerComponentState.Idle:
                        Idle();
                        break;
                    case EServerComponentState.Connecting:
                        Connecting();
                        break;
                    case EServerComponentState.Disabled:
                        Disabled();
                        break;
                }
            }
        }

        public event Func<ServerComponent, Task> OnConnectionRequested;
        public event Func<ServerComponent, Task> OnEditRequested;

        public ServerComponent(IServiceProvider services, SavedServer server) {
            _services = services;

            Server = server;

            InitializeComponent();

            Update();
        }

        public void Update() {
            TextBlock_Name.Text = Server.Name;
            TextBlock_Host.Text = Server.Host.ToString() ?? "";
        }

        private void Idle() {
            TextBlock_Status.Visibility = Visibility.Collapsed;
            StackPanel_Buttons.Visibility = Visibility.Visible;

            _state = EServerComponentState.Idle;
        }
        private void Connecting() {
            TextBlock_Status.Visibility = Visibility.Visible;
            StackPanel_Buttons.Visibility = Visibility.Collapsed;

            TextBlock_Status.Text = "Connecting...";

            _state = EServerComponentState.Connecting;
        }
        private void Disabled() {
            TextBlock_Status.Visibility = Visibility.Collapsed;
            StackPanel_Buttons.Visibility = Visibility.Collapsed;

            _state = EServerComponentState.Disabled;
        }

        private async void Button_Connect_Click(object sender, RoutedEventArgs e) {
            var connectionPage = _services.GetRequiredService<ConnectionPage>();

            await OnConnectionRequested.Invoke(this);
        }

        private async void Button_Edit_Click(object sender, RoutedEventArgs e) {
            await OnEditRequested.Invoke(this);
        }
    }
}
