using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions;

public class PointsLimitException : PamelloException
{
    public PointsLimitException(string? message = null) : base(message ?? "Cannot remove/add any more points") { }
}
