using System.Collections;
using System.Text;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Wrapper.Commands;

namespace PamelloV7.Tools.Generators;

public static class CommandExtensionsGenerator
{
    public static void Generate(DirectoryInfo targetDirectory) {
        var assembly = typeof(PamelloCommand).Assembly;
        
        var commandsType = typeof(PamelloCommandsService);

        var path = Path.Combine(targetDirectory.FullName, $"{commandsType.Name}Extensions.g.cs");
        Console.WriteLine(path);
        
        var commands = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(PamelloCommand))).ToList();

        var sb = new StringBuilder();
        
        sb.AppendLine(
            $$"""
            //auto generated
            
            namespace PamelloV7.Wrapper.Extensions;
            
            public static class {{commandsType.Name}}Extensions {
            """
        );

        foreach (var command in commands) {
            if (command is null) throw new Exception("No PlayerQueueSkip found");
        
            var executeMethod = command.GetMethod("Execute");
            if (executeMethod is null) continue;

            var returnTypeInfo = GetReturnTypeInfo(executeMethod.ReturnType, false);

            Console.WriteLine($"Command: {command.Name}");
            var csArgString = "";
            var pathArgString = "";
            foreach (var parameter in executeMethod.GetParameters()) {
                var parameterTypeInfo = GetReturnTypeInfo(parameter.ParameterType, true);
                Console.WriteLine($"    {parameter.Name}: {parameterTypeInfo}");
                
                if (parameterTypeInfo.IsVoid) continue;
                
                csArgString += $", {parameterTypeInfo} {parameter.Name}";

                pathArgString += pathArgString.Length == 0 ? "?" : "&";
                pathArgString += $"{parameter.Name}={{{parameter.Name}}}";
            }
            
            sb.AppendLine(
                $$"""
                      public static {{(returnTypeInfo.IsVoid ? "Task" : $"Task<{returnTypeInfo}>")}} {{command.Name}}(this {{commandsType.FullName}} commands{{csArgString}}) {
                          return commands.Invoker.ExecuteCommandAsync{{(returnTypeInfo.IsVoid ? "" : $"<{returnTypeInfo}>")}}({{(pathArgString.Length > 0 ? "$" : "")}}"{{command.Name}}{{pathArgString}}");
                      }
                  """
            );
        }
        
        sb.AppendLine("}");
        
        File.WriteAllText(path, sb.ToString());
    }

    public record TranslatedTypeInfo(Type Type, bool IsParameter)
    {
        public Type Type { get; set; } = Type;
        public bool IsEnumerable { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPamelloEntity { get; set; }
        public bool IsParameter { get; } = IsParameter;
        public bool IsVoid => Type == typeof(void);

        public override string ToString() {
            if (IsVoid) {
                if (IsEnumerable) return "IEnumerable";
                return "void";
            }

            if (IsPamelloEntity && !IsParameter) {
                Type = typeof(int);
            }

            if (IsEnumerable) {
                if (IsPamelloEntity && IsParameter) return "string";
                return $"IEnumerable<{Type.FullName}{(IsNullable ? "?" : "")}>";
            }
            
            if (IsPamelloEntity && IsParameter) return "string";
            
            return $"{Type.FullName ?? "void"}{(IsNullable ? "?" : "")}";
        }
    }

    public static TranslatedTypeInfo GetReturnTypeInfo(Type initialType, bool isParameter) {
        TranslatedTypeInfo final = new(initialType, isParameter);
        
        if (final.Type.IsAssignableTo(typeof(Task))) {
            final.Type = initialType.GenericTypeArguments.FirstOrDefault() ?? typeof(void);
        }
        
        if (final.Type.IsGenericType && final.Type.IsAssignableTo(typeof(IEnumerable))) {
            final.IsEnumerable = true;
            final.Type = final.Type.GenericTypeArguments.FirstOrDefault() ?? typeof(void);
        }
        
        if (Nullable.GetUnderlyingType(final.Type) is { } type) {
            final.IsNullable = true;
            final.Type = type;
        }
        
        if (final.Type.IsAssignableTo(typeof(IPamelloEntity))) {
            final.IsPamelloEntity = true;
        }

        return final;
    }
}
