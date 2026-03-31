namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class PamelloEntityAttribute : Attribute
{
    public string ProviderName { get; }
    public Type DtoType { get; set; }
    
    public PamelloEntityAttribute(string providerName, Type dtoType) {
        ProviderName = providerName;
        DtoType = dtoType;
    }
}
