using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class MethodObligationGenerator : IIncrementalGenerator
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

    public static IEnumerable<AttributeData?> GetAttributesInClass(INamedTypeSymbol classSymbol, string attributeName, int depth = 0) {
        if (depth >= 2) yield break;
        
        foreach (var attribute in classSymbol.GetAttributes()) {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null) continue;
            
            if (attributeClass.Name == attributeName) yield return attribute;

            foreach (var data in GetAttributesInClass(attributeClass, attributeName, depth + 1)) {
                yield return data;
            }
        }
    }
    
    private static MethodObligationDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType)
            return null;
        if (PamelloEventGenerator.InheritsFrom(classType, "Attribute"))
            return null;

        var attributes = GetAttributesInClass(classType, "RequiredMethodNameAttribute").ToList();
        if (attributes.Count == 0) return null;

        var methods = attributes.Select(data => data?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString()).OfType<string>().ToArray();
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found in {classType.Name}");
        
        return new MethodObligationDescriptor(
            classType,
            methods,
            debug
        );
    }
    
    private static void Generate(SourceProductionContext context, MethodObligationDescriptor descriptor) {
        var classNamespace = GeneratorBase.GetNamespace(descriptor.ClassType);
        
        var sb = GeneratorBase.WriteInsideClasses(
            descriptor.ClassType,
            $": I{descriptor.ClassType.Name}Obligations",
            "//nothing"
        );
        
        var source =
            $$"""
              /* debug output
              {{descriptor.DebugOutput}}
              */

              namespace {{classNamespace}};

              public interface I{{descriptor.ClassType.Name}}Obligations {
                  {{(
                      string.Join("\n", descriptor.RequiredMethods.Select(method => 
                          (descriptor.ClassType.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == method)
                              ? $"//method \"{method}\" was found"
                              : $"        protected void {method}();"
                          )
                      ))
                  )}}
              }
                
              {{sb}}
              """;
        
        context.AddSource($"{descriptor.ClassType.Name}.Obligations.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
