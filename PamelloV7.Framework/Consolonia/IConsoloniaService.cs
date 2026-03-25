using Avalonia.Controls;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Consolonia;

public interface IConsoloniaService : IPamelloService
{
    public void SetMainScreen();
    public void SetLogScreen();

    public TControl SetScreen<TControl>(Func<TControl> getScreen)
        where TControl : Control;
    public void SetScreen(Control screen);
}
