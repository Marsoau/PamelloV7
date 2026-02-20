namespace PamelloV7.Framework.Entities.Base;

public interface IPamelloDatabaseEntity : IPamelloDynamicEntity
{
    public void Init();
    public void Save();
}
