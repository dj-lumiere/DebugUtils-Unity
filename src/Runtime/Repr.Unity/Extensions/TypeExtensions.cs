#nullable enable
using Type = System.Type;

namespace DebugUtils.Unity.Repr.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsAssignableTo(this Type type, Type other)
        {
            return other.IsAssignableFrom(c: type);
        }
    }
}