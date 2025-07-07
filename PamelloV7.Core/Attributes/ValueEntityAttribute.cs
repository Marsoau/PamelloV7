namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class ValueEntityAttribute : Attribute
{
    public string ProviderName { get; }
    
    public ValueEntityAttribute(string providerName) {
        ProviderName = providerName;
    }
}
