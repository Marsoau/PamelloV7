namespace PamelloV7.Wrapper.Events.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventFullNameAttribute : Attribute
{
    public string Name { get; }
    
    public EventFullNameAttribute(string name) {
        Name = name;
    }
}
