using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PamelloV7.Framework.Consolonia.Screens;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Modules;
using PamelloV7.Server.Consolonia.Controls;
using PamelloV7.Server.Logging;

namespace PamelloV7.Server.Consolonia.Screens;

public partial class MainScreen : PamelloScreen
{
    private readonly IServiceProvider _services;

    private IPamelloLogger Logger { get; set; }
    
    public MainScreen(IServiceProvider services) {
        _services = services;
        
        Logger = services.GetRequiredService<IPamelloLogger>();
        
        InitializeComponent();
        
        DataContext = this;
    }
}
