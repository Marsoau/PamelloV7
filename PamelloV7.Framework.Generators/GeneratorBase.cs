using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators;

public static class GeneratorBase
{
    public static string Tab(int count) => string.Join("", Enumerable.Repeat("    ", count));
    
    public static string GetNamespace(ITypeSymbol classSymbol) {
        return classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();
    }

    public static IEnumerable<string> GetContainingTypes(ITypeSymbol classSymbol) {
        while (true) {
            var containingType = classSymbol.ContainingType;
            if (containingType is null) yield break;
            
            yield return containingType.Name;

            classSymbol = containingType;
        }
    }

    public static void WriteInsideClasses(string[] classNames, string inheritancePart, string innerPart, StringBuilder sb, int depth = 0) {
        if (classNames.Length == 0) return;
        
        sb.Append($"{Tab(depth)}public partial class {classNames[depth]} ");
        
        if (depth + 1 >= classNames.Length) {
            sb.AppendLine($"{inheritancePart} {{");
            
            sb.Append(Tab(depth + 1));
            sb.AppendLine(innerPart.Replace("\n", $"\n{Tab(depth + 1)}"));
            
            sb.AppendLine($"{Tab(depth)}}}");
            return;
        }

        sb.AppendLine($"{{");
        WriteInsideClasses(classNames, inheritancePart, innerPart, sb, depth + 1);
        sb.AppendLine($"{Tab(depth)}}}");
    }
    
    public static StringBuilder WriteInsideClasses(ITypeSymbol type, string inheritancePart, string innerPart) {
        var sb = new StringBuilder();
        
        var containingTypes = GetContainingTypes(type).ToArray();
        WriteInsideClasses([..containingTypes, type.Name], inheritancePart, innerPart, sb);
        
        return sb;
    }
    
    public static bool HasMethod(ITypeSymbol? type, string methodName, StringBuilder? debug = null) {
        while (true) {
            if (type is null) return false;

            debug?.AppendLine($"Checking: {type.Name}");
            foreach (var member in type.GetMembers().OfType<IMethodSymbol>()) {
                debug?.AppendLine($"| {member.Name}");
            }
            
            if (type.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == methodName))
                return true;

            type = type.BaseType;
        }
    }
}
