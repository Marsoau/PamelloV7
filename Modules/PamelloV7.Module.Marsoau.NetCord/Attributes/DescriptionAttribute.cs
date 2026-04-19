using System.Reflection;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class DescriptionAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ParameterInfo Parameter { get; set; } = null!;
    
    public bool IsOptional => Parameter.HasDefaultValue || Parameter.GetCustomAttribute<DefaultQueryAttribute>() is not null;
    
    public DescriptionAttribute(string name, string description) {
        Name = name;
        Description = description;
    }

    public static DescriptionAttribute? GetFromParameter(ParameterInfo parameter) {
        if (parameter.Name is null) return null;
        
        var attribute = parameter.GetCustomAttributes().FirstOrDefault(a => a is DescriptionAttribute) as DescriptionAttribute
            ?? new DescriptionAttribute(parameter.Name, parameter.Name);
            
        attribute.Parameter = parameter;
        
        return attribute;
    }
    
    public static DescriptionAttribute[] GetFromParameters(Type type) {
        var executionMethod = type.GetMethod("Execute");
        if (executionMethod is null) throw new PamelloException("Discord command doesnt have execution method");
        
        var attributes = executionMethod.GetParameters().Select(GetFromParameter).OfType<DescriptionAttribute>();
        
        return attributes.ToArray();
    }
}
