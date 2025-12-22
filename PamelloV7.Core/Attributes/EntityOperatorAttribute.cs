namespace PamelloV7.Core.Attributes;

public class EntityOperatorAttribute : Attribute
{
    public string Name { get; }
    public char Symbol { get; }
    
    public EntityOperatorAttribute(string name, char op) {
        Name = name;
        Symbol = op;
    }
}
