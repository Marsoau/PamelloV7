namespace PamelloV7.Module.Marsoau.PEQL.Model;

public class EntityOperatorDescriptor
{
    public string Name { get; }
    public char Symbol { get; }
    public Type Type { get; }
    
    public EntityOperatorDescriptor(string name, char symbol, Type type)
    {
        Name = name;
        Symbol = symbol;
        Type = type;
    }
}
