namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AddInputAttribute : Attribute
{
    public string Name { get; set; }
    
    public AddInputAttribute(string name) {
        Name = name;
    }
}
