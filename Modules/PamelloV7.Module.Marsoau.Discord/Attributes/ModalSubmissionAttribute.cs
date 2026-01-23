namespace PamelloV7.Module.Marsoau.Discord.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ModalSubmissionAttribute : Attribute
{
    public string Name { get; set; }
    
    public ModalSubmissionAttribute(string name) {
        Name = name;
    }
}
