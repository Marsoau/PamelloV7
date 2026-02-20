namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class ValueEntityAttribute : Attribute
{
    public string ProviderName { get; }
    
    public ValueEntityAttribute(string providerName) {
        ProviderName = providerName;
    }
}
