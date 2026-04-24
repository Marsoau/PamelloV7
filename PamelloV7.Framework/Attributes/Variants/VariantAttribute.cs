namespace PamelloV7.Framework.Attributes.Variants;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class VariantAttribute : Attribute {
    public string Name { get; }
    public bool Intercepted { get; }
        
    public VariantAttribute(string name, bool intercepted = false) {
        Name = name;
        Intercepted = intercepted;
    }
}
