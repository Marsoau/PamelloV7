using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.Discord.Services;

public class GroupDescriptor
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<Type> TargetTypes { get; set; }
    public required List<GroupDescriptor> NestedGroups { get; set; }
}

public class DynamicGroupBuilder : IPamelloService
{
    private readonly IAssemblyTypeResolver _types;
    
    private readonly ModuleBuilder _moduleBuilder;
    
    public readonly List<Type> ModulesTypes;
    
    public DynamicGroupBuilder(IServiceProvider services) {
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
        
        var assemblyName = new AssemblyName("DynamicDiscordModules");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        
        _moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        
        ModulesTypes = [];
    }

    public void Startup(IServiceProvider services) {
        var types = _types.GetInheritorsOf<DiscordCommand>();

        var groups = new List<GroupDescriptor>();

        Console.WriteLine("Found types:");
        foreach (var type in types) {
            var attribute = type.GetCustomAttributes()
                .OfType<DiscordGroupAttribute>()
                .FirstOrDefault();
            if (attribute is null) {
                ModulesTypes.Add(type);
                continue;
            }

            if (string.IsNullOrWhiteSpace(attribute.GroupString) || string.IsNullOrWhiteSpace(attribute.Description)) {
                throw new Exception($"Invalid group attribute {attribute} in {type}");
            }
            
            var subGroups = attribute.GroupString.Split(' ');
            if (subGroups.Length > 2) {
                throw new Exception($"Too many subgroups \"{attribute.GroupString}\" in {attribute}");
            }
            
            var descriptor = GuaranteedGroup(subGroups[0], attribute.Description ?? "none");
            if (subGroups.Length == 2)
                descriptor = GuaranteedGroup(subGroups[1], attribute.Description ?? "none", descriptor.NestedGroups);
            
            descriptor.TargetTypes.Add(type);
        }
        
        ModulesTypes.AddRange(groups.Select(group => BuildGroup(group)));
        return;

        GroupDescriptor GuaranteedGroup(string name, string description, List<GroupDescriptor>? parentGroups = null) {
            parentGroups ??= groups;
            
            var descriptor = parentGroups.FirstOrDefault(d => d.Name == name);
            if (descriptor is not null) return descriptor;
            
            descriptor = new GroupDescriptor {
                Name = name,
                Description = description,
                TargetTypes = [],
                NestedGroups = []
            };
            
            parentGroups.Add(descriptor);
            
            return descriptor;
        }
    }

    public Type BuildGroup(GroupDescriptor descriptor, TypeBuilder? parentBuilder = null) {
        var constructor = typeof(GroupAttribute).GetConstructor([typeof(string), typeof(string)]);
        Debug.Assert(constructor is not null);

        TypeBuilder groupBuilder;

        if (parentBuilder is not null) {
            groupBuilder = parentBuilder.DefineNestedType(
                $"Group_{descriptor.Name.Replace("-", "_")}",
                TypeAttributes.NestedPublic | TypeAttributes.Class,
                typeof(DiscordCommand)
            );
        }
        else {
            groupBuilder = _moduleBuilder.DefineType(
                $"Group_{descriptor.Name.Replace("-", "_")}",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(DiscordCommand)
            );
        }

        groupBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, [descriptor.Name, descriptor.Description]));

        var commandBuilders = new List<TypeBuilder>();
        var index = 0;
        
        foreach (var type in descriptor.TargetTypes) {
            var commandType = groupBuilder.DefineNestedType(
                $"{type.Name}_Command_{index++}",
                TypeAttributes.NestedPublic | TypeAttributes.Class,
                type
            );
            
            commandBuilders.Add(commandType);
        }

        foreach (var group in descriptor.NestedGroups) {
            if (parentBuilder is not null) {
                throw new Exception("Nested groups are not supported in subgroups by discord");
            }
            
            BuildGroup(group, groupBuilder);
        }

        foreach (var commandBuilder in commandBuilders) {
            commandBuilder.CreateType();
        }

        return groupBuilder.CreateType();
    }
}
