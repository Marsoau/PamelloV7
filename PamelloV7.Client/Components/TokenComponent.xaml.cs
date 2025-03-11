using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Enumerators;
using PamelloV7.Client.Pages;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
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
    /// Interaction logic for TokenComponent.xaml
    /// </summary>
    public partial class TokenComponent : UserControl
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        public RemoteUser? User { get; private set; }
        public readonly Guid Token;

        private ETokenComponentState _state;
        public ETokenComponentState State {
            get => _state;
            set {
                switch (value) {
                    case ETokenComponentState.Requesting:
                        Requesting();
                        break;
                    case ETokenComponentState.Ready:
                        Ready();
                        break;
                    case ETokenComponentState.Authorizing:
                        Authorizing();
                        break;
                    case ETokenComponentState.Disabled:
                        Disabled();
                        break;
                }
            }
        }

        public event Func<TokenComponent, Task> OnLoad;
        public event Func<TokenComponent, Task> OnLoadFail;

        public TokenComponent(IServiceProvider services, Guid token) {
            _services = services;

            _pamello = services.GetRequiredService<PamelloClient>();

            Token = token;
            User = null;

            InitializeComponent();
        }

        public void Update() {
            if (User is null) return;

            TextBlock_Name.Text = User.Name;
            TextBlock_Id.Text = $"{User.Id} | {User.DiscordId}";

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(User.AvatarUrl ?? "");
            image.EndInit();

            Image_Avatar.Source = image;
        }

        private void Ready() {
            TextBlock_Status.Visibility = Visibility.Collapsed;
            StackPanel_Buttons.Visibility = Visibility.Visible;

            _state = ETokenComponentState.Ready;
        }
        private void Authorizing() {
            TextBlock_Status.Visibility = Visibility.Visible;
            StackPanel_Buttons.Visibility = Visibility.Collapsed;

            TextBlock_Status.Text = "Authorizing...";

            _state = ETokenComponentState.Authorizing;
        }
        private void Requesting() {
            TextBlock_Status.Visibility = Visibility.Visible;
            StackPanel_Buttons.Visibility = Visibility.Collapsed;

            TextBlock_Status.Text = "Requesting User Info...";

            _state = ETokenComponentState.Requesting;
        }
        private void Disabled() {
            TextBlock_Status.Visibility = Visibility.Collapsed;
            StackPanel_Buttons.Visibility = Visibility.Collapsed;

            _state = ETokenComponentState.Disabled;
        }


        private async void Button_Authorize_Click(object sender, RoutedEventArgs e) {
            var connectionPage = _services.GetRequiredService<AuthorizationPage>();

            await connectionPage.TokenComponent_Button_Authorize_Click(this);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
            Requesting();

            User = await _pamello.Users.GetNew(Token);

            if (User is not null) {
                Update();
                if (OnLoad is not null) await OnLoad.Invoke(this);
            }
            else {
                if (OnLoadFail is not null) await OnLoadFail.Invoke(this);
            }

            Ready();
        }
    }
}
