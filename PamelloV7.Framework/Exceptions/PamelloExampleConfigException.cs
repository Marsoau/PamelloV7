namespace PamelloV7.Framework.Exceptions;

public class PamelloExampleConfigException : PamelloLoadingException
{
    public PamelloExampleConfigException() : base("Awaiting real config file") { }
}
