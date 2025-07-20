namespace PamelloV7.Core.Model.Entities.Base;

public interface IPamelloDatabaseEntity : IPamelloEntity
{
    public void Init();
    public void Save();
}
