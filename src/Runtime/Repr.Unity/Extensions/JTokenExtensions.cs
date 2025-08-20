#nullable enable
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Extensions
{
    internal static class JTokenExtensions
    {
        public static JValue ToJValue(this string str)
        {
            return new JValue(value: str);
        }

        public static JValue ToJValue(this int i)
        {
            return new JValue(value: i);
        }

        public static JValue ToJValue(this int? i)
        {
            return i == null
                ? JValue.CreateNull()
                : ToJValue(i: i.Value);
        }
    }
}