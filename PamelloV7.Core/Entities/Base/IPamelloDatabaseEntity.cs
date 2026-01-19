namespace PamelloV7.Core.Entities.Base;

public interface IPamelloDatabaseEntity : IPamelloEntity
{
    public void Init();
    public void Save();
}
