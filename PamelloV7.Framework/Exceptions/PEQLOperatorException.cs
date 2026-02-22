using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions;

public class PEQLOperatorException : PamelloException
{
    public PEQLOperatorException(string? message = "") : base(message) { }
}
