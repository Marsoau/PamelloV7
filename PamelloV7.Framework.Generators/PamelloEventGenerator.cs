using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class PamelloEventGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<EventClassDescriptor> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 }, 
                transform: GetDescriptor
            )
            .Where(static m => !string.IsNullOrEmpty(m.ClassName));

        context.RegisterSourceOutput(classDeclarations, ExecuteGeneration);
    }

    private static EventClassDescriptor GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) is not INamedTypeSymbol classSymbol) {
            return default;
        }

        var implementsPamelloEvent = InheritsFrom(classSymbol, "IPamelloEvent");
        var implementsRevertible = InheritsFrom(classSymbol, "IRevertiblePamelloEvent");
        
        if (!implementsPamelloEvent && !implementsRevertible) return default;
        
        var baseImplementsPamelloEvent = InheritsFrom(classSymbol.BaseType, "IPamelloEvent");
        var baseImplementsRevertible = InheritsFrom(classSymbol.BaseType, "IRevertiblePamelloEvent");


        var hasInvoker = classSymbol.GetMembers("Invoker").Any();
        var hasRevertPack = classSymbol.GetMembers("RevertPack").Any();
        var hasDefaultPack = false;
        
        if (implementsRevertible) {
            var nestedTypes = classSymbol.GetTypeMembers("Pack");
            
            if (nestedTypes.Length > 0) {
                if (nestedTypes[0] is { IsAbstract: false, Arity: 0 } packType) {
                    hasDefaultPack = packType.Constructors.Any(c => c.Parameters.Length == 0 && !c.IsStatic);
                }
            }
        }
        
        //info update

        var updateEntries = new List<EventClassInfoUpdateEntry>();
        
        var debugSb = new StringBuilder();

        foreach (var attribute in classSymbol.GetAttributes()) {
            /*
            if (attribute.AttributeClass is not { Name: "EntityInfoUpdateAttribute" } attributeClass) continue;
               
            debugSb.AppendLine("at 0");
               
            if (attribute.ConstructorArguments is not {
                Length: >= 2
            } constructorArguments) continue;
               
            var entityPropertyName = constructorArguments.ElementAt(0).Value?.ToString();
            var updatePropertyPath = constructorArguments.ElementAt(1).Values.Select(v => v.Value?.ToString()).ToArray();
            */
            if (attribute.AttributeClass is not { Name: "EntityInfoUpdateAttribute" } attributeClass) continue;
            if (attributeClass.TypeArguments.FirstOrDefault() is not { } entityTypeSymbol) continue;
            
            debugSb.AppendLine("at 0");
            
            if (entityTypeSymbol.GetAttributes().FirstOrDefault(
                a => a.AttributeClass is { Name: "PamelloEntityAttribute" }
            ) is not { } pamelloEntityAttributeData) continue;
            
            debugSb.AppendLine("at 1");
            
            if (attribute.ConstructorArguments is not {
                Length: >= 2
            } constructorArguments) continue;
            
            var entityPropertyName = constructorArguments.ElementAt(0).Value?.ToString()!;
            var updatePropertyPath = constructorArguments.ElementAt(1).Values.Select(v => v.Value?.ToString()!).ToArray();
            
            debugSb.AppendLine($"at 2:1 {string.Join(".", updatePropertyPath)}");
            
            if (pamelloEntityAttributeData.ConstructorArguments is not {
                Length: >= 2
            } entityArguments || entityArguments.ElementAt(1) is not {
                Value: ITypeSymbol dtoType
            }) continue;
            
            debugSb.AppendLine($"at 2 {dtoType.Name}");
            
            
            
            updateEntries.Add(new EventClassInfoUpdateEntry(dtoType, entityPropertyName, updatePropertyPath));
        }
        
        //end

        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();

        return new EventClassDescriptor(
            namespaceName, 
            classSymbol.Name, 
            NeedsInvoker: implementsPamelloEvent && !baseImplementsPamelloEvent && !hasInvoker, 
            NeedsRevertPack: implementsRevertible && !baseImplementsRevertible && !hasRevertPack,
            hasDefaultPack,
            updateEntries.ToArray(),
            debugSb
        );
    }

    private static bool InheritsFrom(INamedTypeSymbol? classSymbol, string targetName) {
        if (classSymbol == null) return false;
        if (classSymbol.AllInterfaces.Any(i => i.Name == targetName)) return true;

        var currentBaseType = classSymbol.BaseType;
        while (currentBaseType != null) {
            if (currentBaseType.Name == targetName) return true;
            currentBaseType = currentBaseType.BaseType;
        }

        return false;
    }

    private static void ExecuteGeneration(SourceProductionContext context, EventClassDescriptor eventClass) {
        if (eventClass is { NeedsInvoker: false, NeedsRevertPack: false }) return;
        
        var infoUpdatePropertiesSb = new StringBuilder();

        foreach (var infoUpdateEntry in eventClass.UpdateEntries) {
            IPropertySymbol? property = null;
            var list = new List<IPropertySymbol>();
            
            var targetType = infoUpdateEntry.DtoType;

            var currentPart = 0;
            while (targetType is not null && infoUpdateEntry.UpdatePropertyPath.Length > currentPart) {
                list.Clear();
                list.AddRange(targetType.BaseType?.GetMembers().OfType<IPropertySymbol>() ?? []);
                list.AddRange(targetType.GetMembers().OfType<IPropertySymbol>());
                var members = list.ToArray();
            
                property = members.FirstOrDefault(p => p.Name == infoUpdateEntry.UpdatePropertyPath.ElementAt(currentPart));
                targetType = property?.Type;
                
                currentPart++;
            }
            
            eventClass.DebugOutput.AppendLine(
                $"info update for {string.Join(".", infoUpdateEntry.UpdatePropertyPath)}: {property?.Name ?? "nema"}"
            );
            
            if (property is null) continue;

            infoUpdatePropertiesSb.AppendLine(
                $"        public required {property.Type.GetFullName()} {property.Name} {{ get; set; }}"
            );
        }

        var source =
            $$"""
              // <auto-generated/>
              #nullable enable
              
              /* debug:
              {{eventClass.DebugOutput}}
              */
              //test: {{eventClass.UpdateEntries.Length}}

              using PamelloV7.Framework.Entities;
              using PamelloV7.Framework.Events.RestorePacks.Base;
              using PamelloV7.Framework.Events.Base;
              using PamelloV7.Framework.Containers;
              using System.Text.Json.Serialization;
              
              namespace {{eventClass.Namespace}}
              {
                  partial class {{eventClass.ClassName}}
                  {
              {{infoUpdatePropertiesSb}}
                      {{(eventClass.NeedsInvoker ? 
              """
              public readonly SafeStoredEntity<IPamelloUser> _safeInvoker = new(0);
                      public IPamelloUser? Invoker {
                          get => _safeInvoker.Entity!;
                          set => _safeInvoker.Entity = value;
                      }
              """ : "//invoker is not needed")}}
              
                      {{(!eventClass.HasDefaultPack ? "" : $"partial class Pack : RevertPack<{eventClass.ClassName}>;")}}
                      {{(eventClass.NeedsRevertPack ? $"[JsonIgnore] public{(eventClass.HasDefaultPack ? " " : " required ")}IRevertPack RevertPack {{ get; set; }}{(
                          eventClass.HasDefaultPack ? " = new Pack();" : " //default pack is not needed"
                      )}" : "//revert pack is not needed")}}
                  }
              }
              """;

        context.AddSource($"{eventClass.ClassName}.Event.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}