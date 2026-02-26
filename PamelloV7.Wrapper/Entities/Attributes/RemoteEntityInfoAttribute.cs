namespace PamelloV7.Wrapper.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class RemoteEntityInfoAttribute : Attribute
{
    public string ProviderName { get; }
    public string RemoteInterfaceName { get; set; }
    public Type DtoType { get; set; }
    
    protected RemoteEntityInfoAttribute(string providerName, string remoteInterfaceName, Type dtoType) {
        ProviderName = providerName;
        RemoteInterfaceName = remoteInterfaceName;
        DtoType = dtoType;
    }
}
