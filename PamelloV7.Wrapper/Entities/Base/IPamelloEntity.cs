namespace PamelloV7.Wrapper.Entities.Base;

public interface IPamelloEntity
{
    public int Id { get; }
    public string Name { get; }

    internal Type GetDtoType();
}
