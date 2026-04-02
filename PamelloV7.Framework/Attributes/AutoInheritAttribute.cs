namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoInheritAttribute : Attribute
{
    public Type? ClassType { get; }
    public Type[]? InterfaceTypes { get; }
    
    public AutoInheritAttribute(Type? classType, params Type[]? interfaceTypes) {
        ClassType = classType;
        InterfaceTypes = interfaceTypes;
    }
}
