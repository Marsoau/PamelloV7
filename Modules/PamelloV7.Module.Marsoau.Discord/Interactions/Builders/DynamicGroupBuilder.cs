using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Builders;

public class GroupDescriptor
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Type TargetType { get; set; }
}

public class DynamicGroupBuilder : IPamelloService
{
    private readonly IAssemblyTypeResolver _types;
    
    private readonly ModuleBuilder _moduleBuilder;
    
    public DynamicGroupBuilder(IServiceProvider services) {
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
        
        var assemblyName = new AssemblyName("DynamicDiscordModules");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        
        _moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
    }

    public void Startup(IServiceProvider services) {
        throw new NotImplementedException();
    }

    public Type BuildSingleGroup(string groupName, string groupDesc, Type targetType) {
        var constructor = typeof(GroupAttribute).GetConstructor([typeof(string), typeof(string)]);
        Debug.Assert(constructor is not null);

        var classBuilder = _moduleBuilder.DefineType(
            $"DynamicSingleGroup_{groupName.Replace("-", "_")}_{targetType.Name}",
            TypeAttributes.Public | TypeAttributes.Class,
            targetType
        );

        classBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, [groupName, groupDesc]));

        return classBuilder.CreateType();
    }
    
    public Type BuildDoubleGroup(string rootGroupName, string rootGroupDesc, IEnumerable<GroupDescriptor> subGroups) {
        var constructor = typeof(GroupAttribute).GetConstructor([typeof(string), typeof(string)]);
        Debug.Assert(constructor is not null);

        var mainBuilder = _moduleBuilder.DefineType(
            $"DynamicRoot_{rootGroupName.Replace("-", "_")}",
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(DiscordCommand)
        );

        mainBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, [rootGroupName, rootGroupDesc]));

        var nestedBuilders = new List<TypeBuilder>();
        var index = 0;
        foreach (var sub in subGroups) {
            var nestedBuilder = mainBuilder.DefineNestedType(
                $"{sub.TargetType.Name}_SubGroup_{index}",
                TypeAttributes.NestedPublic | TypeAttributes.Class,
                sub.TargetType 
            );

            nestedBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, [sub.Name, sub.Description]));
            
            nestedBuilders.Add(nestedBuilder);
            index++;
        }

        foreach (var nested in nestedBuilders) {
            nested.CreateType();
        }

        return mainBuilder.CreateType();
    }
}