﻿using System;
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
using PamelloV7.Client.Interfaces;
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

            _pamello.Events.OnConnect += Events_OnConnection;
            _pamello.Events.OnDisconnect += Events_OnDisconnect;

            _pamello.Events.OnEventsAuthorized += Events_OnEventsAuthorized;
            _pamello.Events.OnEventsUnAuthorized += Events_OnEventsUnAuthorized;

            InitializeComponent();
        }

        private async Task Events_OnDisconnect() {
            Console.WriteLine("--- OnDisconnect");
            Dispatcher.Invoke(SwitchPage<ConnectionPage>);
        }

        private async Task Events_OnEventsUnAuthorized(Core.Events.EventsUnAuthorized arg) {
            Console.WriteLine("--- OnUnauthorized");
            Dispatcher.Invoke(SwitchPage<AuthorizationPage>);
        }

        private async Task Events_OnEventsAuthorized(Core.Events.EventsAuthorized arg) {
            Console.WriteLine("--- OnAuthorized");
            Dispatcher.Invoke(SwitchPage<MainPage>);
        }

        private async Task Events_OnConnection() {
            Console.WriteLine("--- OnConnect");
            Dispatcher.Invoke(SwitchPage<AuthorizationPage>);
        }

        public TPage SwitchPage<TPage>() where TPage : Page {
            var page = _services.GetRequiredService<TPage>();

            Content = page;

            return page;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                if (Content is IRefrashable page) {
                    if (Keyboard.IsKeyDown(Key.LeftShift)) {
                        page.Update();
                    }
                    else {
                        page.Refresh();
                    }
                }
            }
        }
    }
}
