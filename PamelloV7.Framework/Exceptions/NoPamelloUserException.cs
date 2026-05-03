using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions;

public class NoPamelloUserException : PamelloException
{
    public NoPamelloUserException(string message = "User is required for this operation") : base(message) { }
}
