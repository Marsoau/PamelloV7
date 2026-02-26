namespace PamelloV7.Wrapper.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RemoteEntityAttribute<TDtoType> : RemoteEntityInfoAttribute
{
    public RemoteEntityAttribute(string providerName, string remoteInterfaceName)
        : base(providerName, remoteInterfaceName, typeof(TDtoType))
    { }
}
