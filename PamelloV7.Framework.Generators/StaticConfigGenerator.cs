using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class StaticConfigGenerator : IIncrementalGenerator
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

    private static ConfigRootPartDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType) {
            return null;
        }
        if (classType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "ConfigRootAttribute") is not { } attribute) {
            return null;
        }
        
        var namespaceName = classType.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classType.ContainingNamespace.ToDisplayString();
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found {classType.Name}");
        
        return new ConfigRootPartDescriptor(
            classType,
            debug
        );
    }

    private static string Tab(int count) => string.Join("", Enumerable.Repeat("    ", count));
    private static string AdjustedName(string name, string end) => name.EndsWith(end) ? name.Substring(0, name.Length - end.Length) : name;
    
    private static string GenerateNode(ITypeSymbol nodeType, int depth, bool isRoot = true) {
        
        var innerTypes = nodeType.GetTypeMembers().Where(t => !t.IsAbstract && t.Arity == 0).ToList();
        if (!innerTypes.Any()) return $"{Tab(depth)}//no inner types";
        
        var sb = new StringBuilder();
        
        sb.AppendLine($"{Tab(depth)}public{(isRoot ? " static " : " ")}partial class {nodeType.Name} {{");
        
        foreach (var innerType in innerTypes) {
            sb.AppendLine(Tab(depth + 1) + $"public{(isRoot ? " static " : " readonly ")}{innerType.Name} {AdjustedName(innerType.Name, "Node")} = new();");
            sb.AppendLine(GenerateNode(innerType, depth + 1, false));
        }
        
        sb.AppendLine($"{Tab(depth)}}}");
        
        return sb.ToString();
    }
        
    private static void Generate(SourceProductionContext context, ConfigRootPartDescriptor descriptor) {
        var classNamespace = descriptor.ClassType.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : descriptor.ClassType.ContainingNamespace.ToDisplayString();

        var source =
            $$"""
            /* debug output
            {{descriptor.DebugOutput}}
            */
            
            namespace {{classNamespace}};
            
            //root node
            {{GenerateNode(descriptor.ClassType, 0)}}
            
            """;
        
        context.AddSource($"{descriptor.ClassType.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
