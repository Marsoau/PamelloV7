namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiredMethodNameAttribute : Attribute
{
    public string Name { get; }
    
    public RequiredMethodNameAttribute(string name) {
        Name = name;
    }
}
