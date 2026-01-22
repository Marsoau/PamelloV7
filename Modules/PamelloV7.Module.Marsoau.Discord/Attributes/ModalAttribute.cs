namespace PamelloV7.Module.Marsoau.Discord.Attributes;

public class ModalAttribute : Attribute
{
    public string Name { get; set; }
    
    public ModalAttribute(string name) {
        Name = name;
    }
}
