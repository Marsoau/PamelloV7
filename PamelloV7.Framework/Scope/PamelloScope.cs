using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.Scope;

public static class PamelloScope
{
    private static readonly AsyncLocal<IPamelloUser?> CurrentScopeUser = new();
    
    public static void RequireUser() {
        if (User is null) throw new NoPamelloUserException();
    }

    public static IPamelloUser RequiredUser => User ?? throw new NoPamelloUserException();
    public static IPamelloUser? User => CurrentScopeUser.Value;
    
    public static void SetUser(IPamelloUser? user) => CurrentScopeUser.Value = user;

    public static void SetUserIn(IPamelloUser? user, Action action) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            action();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
    
    public static T SetUserIn<T>(IPamelloUser? user, Func<T> func) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            return func();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
}
