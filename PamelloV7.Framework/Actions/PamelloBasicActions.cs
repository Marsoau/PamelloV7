using System.ComponentModel;
using System.Reflection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Actions;

public class PamelloBasicActions
{
    public static Task<object?> RunExecuteMethodAsync(object? obj, object?[]? arguments = null, Action<Exception>? exceptionHandler = null) {
        return RunMethodAsync("Execute", obj, arguments, exceptionHandler);
    }

    public static async Task<object?> RunMethodAsync(string methodName, object? obj, object?[]? arguments = null, Action<Exception>? exceptionHandler = null) {
        if (obj is null) return null;
        
        var method = obj.GetType().GetMethod(methodName);
        if (method is null) throw new PamelloException($"Could not find \"{methodName}\" method");
        
        return await RunMethodAsync(method, obj, arguments, exceptionHandler);
    }
    public static async Task<object?> RunMethodAsync(MethodInfo method, object obj, object?[]? arguments = null, Action<Exception>? exceptionHandler = null) {
        exceptionHandler ??= x => throw x;

        try {
            object? result;
            
            if (typeof(Task).IsAssignableFrom(method.ReturnType)) {
                if (method.ReturnType.IsGenericType) {
                    result = await (dynamic)method.Invoke(obj, arguments ?? [])!;
                }
                else {
                    await (Task)method.Invoke(obj, arguments ?? [])!;
                    result = null;
                }
            }
            else {
                result = method.Invoke(obj, arguments ?? []);
            }
            
            return result;
        }
        catch (Exception x) {
            exceptionHandler(x);
            return null;
        }
    }

    public static async Task<TType> InTypeFromStringAsync<TType>(string str, string defaultQuery, IEntityQueryService peql, IPamelloUser scopeUser) {
        var value = await InTypeFromStringAsync(typeof(TType), str, defaultQuery, peql, scopeUser);
        return (TType)value!;
    }
    
    public static async Task<object?> InTypeFromStringAsync(Type type, string str, string defaultQuery, IEntityQueryService peql, IPamelloUser scopeUser) {
        var query = string.IsNullOrEmpty(str) ? defaultQuery : str;

        if (type.IsAssignableTo(typeof(IPamelloEntity))) {
            return await peql.ReflectiveGetSingleAsync(type, query, scopeUser);
        }
        
        if (
            type.IsGenericType && (
            type.GetGenericTypeDefinition() == typeof(List<>) ||
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) &&
            type.GenericTypeArguments.First().IsAssignableTo(typeof(IPamelloEntity))
        ) {
            return await peql.ReflectiveGetAsync(type.GenericTypeArguments.First(), query, scopeUser);
        }
        
        var converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(str);
    }
}
