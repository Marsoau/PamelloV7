namespace PamelloV7.Core.Entities.Base;

public interface IPamelloDatabaseEntity : IPamelloDynamicEntity
{
    public void Init();
    public void Save();
}
