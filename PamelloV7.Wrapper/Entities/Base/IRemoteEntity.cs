namespace PamelloV7.Wrapper.Entities.Base;

public interface IRemoteEntity
{
    public int Id { get; }
    public string Name { get; }

    internal Type GetDtoType();
}
