using System.Reflection;

namespace Distributed.Core.Extensions;
public static class ReflectionExtensions
{
    public static async Task<object?> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
    {
        dynamic? awaitable = @this.Invoke(obj, parameters);
        await awaitable;

        return awaitable
            ?.GetAwaiter()
            .GetResult();
    }
}