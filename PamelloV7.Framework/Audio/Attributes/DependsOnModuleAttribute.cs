using PamelloV7.Framework.Audio.Modules.Base;

namespace PamelloV7.Framework.Audio.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnModuleAttribute<TAudioModule> : Attribute
    where TAudioModule : class, IAudioModule
{
    public string Name { get; }
    public string Description { get; }
    
    public DependsOnModuleAttribute(string name, string description = "") {
        Name = name;
        Description = description;
    }
}
