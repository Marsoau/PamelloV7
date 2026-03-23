using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Server.Consolonia.Controls;

public partial class LogMessageControl : UserControl
{
    public LogMessageControl(object? obj, IPamelloModule? module, ELogLevel level) {
        InitializeComponent();

    }
}
