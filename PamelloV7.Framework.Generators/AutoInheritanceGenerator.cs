using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class AutoInheritanceGenerator : IIncrementalGenerator
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

    public static AttributeData? GetAttributeInClass(INamedTypeSymbol classSymbol, string attributeName, int depth = 0) {
        if (depth >= 2) return null;
        
        foreach (var attribute in classSymbol.GetAttributes()) {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null) continue;
            
            if (attributeClass.Name == attributeName) return attribute;
            
            var data = GetAttributeInClass(attributeClass, attributeName, depth + 1);
            if (data is not null) return data;
        }
        
        return null;
    }
    
    private static AutoInheritanceDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType) {
            return null;
        }
        if (classType.BaseType is not null && classType.BaseType.Name != "Object") {
            return null;
        }

        var attribute = GetAttributeInClass(classType, "AutoInheritAttribute");
        if (attribute is null) return null;
        
        var namespaceName = classType.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classType.ContainingNamespace.ToDisplayString();
        
        var type = attribute.ConstructorArguments.ElementAtOrDefault(0).Value as ITypeSymbol;
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found in {classType.Name}");
        
        return new AutoInheritanceDescriptor(
            classType,
            type,
            [],
            debug
        );
    }

    
    private static void Generate(SourceProductionContext context, AutoInheritanceDescriptor descriptor) {
        var classNamespace = GeneratorBase.GetNamespace(descriptor.ClassType);
        
        var sb = GeneratorBase.WriteInsideClasses(
            descriptor.ClassType,
            descriptor.InheritFromType is not null
                ? $" : {descriptor.InheritFromType.GetFullName()}"
                : "",
            "//nothing"
        );

        var source =
            $$"""
              /* debug output
              {{descriptor.DebugOutput}}
              {{descriptor.InheritFromType?.GetFullName()}}
              */
              
              namespace {{classNamespace}};
              
              {{sb}}
              """;
        
        context.AddSource($"{descriptor.ClassType.Name}.AutoInheritance.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
