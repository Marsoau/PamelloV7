using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;

namespace PamelloV7.Framework.Generators;

[Generator]
public class AudioModulesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 }, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);
        
        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
    }

    private static AudioModuleDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType) {
            return null;
        }
        if (classType.IsAbstract || !classType.AllInterfaces.Any(i => i.Name == "IAudioModule")) return null;
        
        var debug = new StringBuilder();
        
        var interfaces = classType.AllInterfaces.Select(i => i.Name).ToList();
        
        debug.AppendLine($"Found audio module: {classType.Name}");
        debug.AppendLine($"Interfaces: {string.Join(", ", interfaces)}");
        
        var descriptor = new AudioModuleDescriptor(
            classType,
            debug
        );

        descriptor.HasSingleInput = interfaces.Contains("IAudioModuleWithInput");
        descriptor.HasSingleOutput = interfaces.Contains("IAudioModuleWithOutput");
        
        return descriptor;
    }

    private static void Generate(SourceProductionContext context, AudioModuleDescriptor descriptor) {
        var classNamespace = descriptor.ClassType.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : descriptor.ClassType.ContainingNamespace.ToDisplayString();
        
        var source =
            $$"""
            /*
            Debug output:
            
            {{descriptor.DebugOutput}}
            */
            
            namespace {{classNamespace}};
            
            public partial class {{descriptor.ClassType.Name}} {
            {{(descriptor.HasSingleInput ?
            $$"""
                public override int MinInputs => 1;
                public override int MaxInputs => 1;
            """ : "//Has its own MinMax inputs")}}
            {{(descriptor.HasSingleOutput ?
            $$"""
                public override int MinOutputs => 1;
                public override int MaxOutputs => 1;
            """ : "//Has its own MinMax outputs")}}
                
                {{descriptor.GetSingleInput()}}
                {{descriptor.GetSingleOutput()}}
            }
            """;
        context.AddSource($"{descriptor.ClassType.Name}.Audio.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
