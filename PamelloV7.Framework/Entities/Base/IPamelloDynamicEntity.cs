namespace PamelloV7.Framework.Entities.Base;

public interface IPamelloDynamicEntity : IPamelloEntity
{
    public string SetName(string name, IPamelloUser scopeUser);
    
    public bool IsChangesGoing { get; }
    
    public void StartChanges();
    public void EndChanges();
}
