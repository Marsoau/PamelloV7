using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Components;
using PamelloV7.Client.Model;
using PamelloV7.Wrapper;
using System.Windows;
using System.Windows.Controls;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        private SavedServer? _savedServer;
        public SavedServer? Server {
            get => _savedServer;
            set {
                _savedServer = value;

                UpdateTokenList();
            }
        }

        public AuthorizationPage(IServiceProvider services) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            InitializeComponent();
        }

        public void UpdateTokenList() {
            StackPanel_TokenList.Children.Clear();

            if (Server is null) return;

            TokenComponent tokenComponent;
            foreach (var token in Server.Tokens) {
                tokenComponent = new TokenComponent(_services, token);
                tokenComponent.Margin = new System.Windows.Thickness(0, 0, 0, 4);

                StackPanel_TokenList.Children.Add(tokenComponent);
            }
        }

        internal async Task TokenComponent_Button_Authorize_Click(TokenComponent tokenSender) {
            foreach (TokenComponent tokenComponent in StackPanel_TokenList.Children) {
                if (tokenComponent != tokenSender) tokenComponent.State = Enumerators.ETokenComponentState.Disabled;
            }

            tokenSender.State = Enumerators.ETokenComponentState.Authorizing;

            await _pamello.Authorization.WithTokenAsync(tokenSender.Token, false);
            await AuthorizeEvents();

            foreach (TokenComponent tokenComponent in StackPanel_TokenList.Children) {
                tokenComponent.State = Enumerators.ETokenComponentState.Ready;
            }
        }

        private async void Button_Authorize_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (!int.TryParse(TextBox_Code.Text, out var code)) {
                MessageBox.Show($"String \"{TextBox_Code.Text}\" is not an integer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (await _pamello.Authorization.WithCodeAsync(code, false)) {
                Console.WriteLine("authorized user with code");
            }
            else {
                MessageBox.Show($"Code is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            await AuthorizeEvents();
        }

        private async Task AuthorizeEvents() {
            if (await _pamello.Events.Authorize()) {
                Console.WriteLine("events authorized");
            }
            else {
                Console.WriteLine("noauthorization((");
            }
        }
    }
}
