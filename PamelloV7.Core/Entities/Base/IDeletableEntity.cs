namespace PamelloV7.Core.Entities.Base;

public interface IDeletableEntity : IEntity
{
    public bool IsDeleted { get; set; }
}
