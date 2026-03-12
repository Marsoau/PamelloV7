using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions;

public class OsUnsupportedException : PamelloException
{
    public OsUnsupportedException() : base("Your os is not supported") { }
}
