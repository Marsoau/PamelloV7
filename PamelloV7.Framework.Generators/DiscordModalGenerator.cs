using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class DiscordModalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 }, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
    }

    private static INamedTypeSymbol? FindInBase(INamedTypeSymbol? classSymbol, string name) {
        while (true) {
            if (classSymbol is null) return null;
            if (classSymbol.Name == name) return classSymbol;
            
            classSymbol = classSymbol.BaseType;
        }
    }
    
    private static DiscordModalDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType)
            return null;
        if (!classType.GetAttributes().Any(a => a.AttributeClass?.Name == "DiscordModalAttribute"))
            return null;

        var debug = new StringBuilder();
        
        var innerType = classType.GetTypeMembers().FirstOrDefault(t => !t.IsAbstract && t.Arity == 0);
        debug.AppendLine($"Nested class: {innerType}");

        debug.AppendLine();
        
        debug.AppendLine($"Attributes:");
        
        var attributes = classType.GetAttributes().AddRange(innerType?.GetAttributes() ?? []);
        var propertiesDescriptors = attributes
            .Select(data => {
                if (data.AttributeClass is not { } attributeClass) return null;
                
                var baseClass = FindInBase(attributeClass, "AddModalPropertyAttribute");
                if (baseClass is null) return null;

                var name = data.ConstructorArguments.FirstOrDefault().Value?.ToString();
                if (name is null || string.IsNullOrEmpty(name)) return null;
                
                name = name.Replace("*", "");
                
                var propertyType = baseClass.TypeArguments.ElementAtOrDefault(0);
                var valueType = baseClass.TypeArguments.ElementAtOrDefault(1);
                
                if (propertyType is null || valueType is null) return null;
                
                return new DiscordModalPropertyDescriptor(propertyType, valueType, name);
            })
            .OfType<DiscordModalPropertyDescriptor>()
            .ToArray();
        
        foreach (var descriptor in propertiesDescriptors) {
            debug.AppendLine($"| {descriptor.Name}");
            debug.AppendLine($"|   {descriptor.PropertiesType.Name}");
            debug.AppendLine($"|   {descriptor.ValueType.Name}");
        }
        
        return new DiscordModalDescriptor(
            classType,
            innerType,
            propertiesDescriptors,
            debug
        );
    }

    private static void Generate(SourceProductionContext context, DiscordModalDescriptor descriptor) {
        var classNamespace = descriptor.ModalClass.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : descriptor.ModalClass.ContainingNamespace.ToDisplayString();
        
        var modalPropertiesBuilder = new StringBuilder();
        var modalBuilderPropertiesBuilder = new StringBuilder();

        foreach (var property in descriptor.Properties) {
            modalPropertiesBuilder.Append($"\n        public {property.ValueType.GetFullName()} {property.Name} {{ get; set; }}");
            modalBuilderPropertiesBuilder.Append($"\n        public {property.PropertiesType.GetFullName()} {property.Name} {{ get; set; }}");
        }

        var source =
            $$"""
              using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
              using NetCord.Rest;

              namespace {{classNamespace}};

              public partial class {{descriptor.ModalClass.Name}} {
                  {{modalPropertiesBuilder}}
                  
                  public partial class Builder : DiscordModalBuilder {
                      {{modalBuilderPropertiesBuilder}}
                      
                      {{(GeneratorBase.HasMethod(descriptor.BuilderClass, "Build", descriptor.DebugOutput)
                          ? "//Build method found"
                          : """public void Build() { if (!Properties.Any()) Properties.AddComponents(new TextDisplayProperties("Empty Modal")); }"""
                      )}}
                  }
              }
              
              /* debug output
              {{descriptor.DebugOutput}}
              */
              """;
        
        context.AddSource($"{descriptor.ModalClass.Name}.DiscordModal.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
