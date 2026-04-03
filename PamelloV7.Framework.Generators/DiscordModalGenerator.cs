using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;

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
    
    private static DiscordModalDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType)
            return null;
        if (!classType.GetAttributes().Any(a => a.AttributeClass?.Name == "DiscordModalAttribute"))
            return null;

        var attributes = classType.GetAttributes().Where(a => a.AttributeClass?.Name.StartsWith("Add") ?? false).ToList();
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found modal in {classType.Name}");

        debug.AppendLine($"Attributes: ({attributes.Count})");
        foreach (var attribute in attributes) {
            debug.AppendLine($"| {attribute.AttributeClass?.Name}");
        }
        
        var innerType = classType.GetTypeMembers().FirstOrDefault(t => !t.IsAbstract && t.Arity == 0);
        debug.AppendLine($"Nested class: {innerType}");
        
        return new DiscordModalDescriptor(
            classType,
            innerType,
            [],
            debug
        );
    }

    private static void Generate(SourceProductionContext context, DiscordModalDescriptor descriptor) {
        var classNamespace = descriptor.ModalClass.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : descriptor.ModalClass.ContainingNamespace.ToDisplayString();

        var source =
            $$"""
              using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
              using NetCord.Rest;

              namespace {{classNamespace}};

              public partial class {{descriptor.ModalClass.Name}} {
                  public partial class Builder : DiscordModalBuilder {
                      {{(GeneratorBase.HasMethod(descriptor.BuilderClass, "Build", descriptor.DebugOutput)
                          ? "//Build method found"
                          : "public void Build() => Properties.AddComponents(new TextDisplayProperties(\"Empty Modal\"));"
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
