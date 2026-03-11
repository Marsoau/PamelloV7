using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Framework.Exceptions;

public class ModuleStartupException : PamelloException
{
    public readonly IPamelloModule Module;
    
    public ModuleStartupException(IPamelloModule module, string? message) : base(message) {
        Module = module;
    }
}
