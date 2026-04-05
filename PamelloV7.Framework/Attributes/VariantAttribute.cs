namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class VariantAttribute : Attribute {
    public string Name { get; }
        
    public VariantAttribute(string name) {
        Name = name;
    }
}
