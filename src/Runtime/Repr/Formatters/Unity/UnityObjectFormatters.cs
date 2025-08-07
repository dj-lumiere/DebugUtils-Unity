using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using Color = UnityEngine.Color;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Unity
{
    internal class ColorFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Color)obj;
            return
                $"Color(R{(int)(t.r * 255)}G{(int)(t.g * 255)}B{(int)(t.b * 255)}A{(int)(t.a * 255)})";
        }
    }

    // internal class Vector2Formatter
    // {
    //     private static string ToRepr(this Vector2 t)
    //     {
    //         return $"Vector2({t.x:F2},{t.y:F2})";
    //     }
    // }
    //
    // private static string ToRepr(this Vector3 t)
    // {
    //     return $"Vector3({t.x:F2},{t.y:F2},{t.z:F2})";
    // }
    //
    // private static string ToRepr(this Vector4 t)
    // {
    //     return $"Vector4({t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2})";
    // }
    //
    // private static string ToRepr(this Quaternion t)
    // {
    //     return $"Quaternion({t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2})";
    // }
    //
    // private static string ToRepr(this Color32 t)
    // {
    //     return $"Color32(R{t.r:X2}G{t.g:X2}B{t.b:X2}A{t.a:X2})";
    // }
    //
    // private static string ToRepr(this GameObject t)
    // {
    //     return $"GameObject {t.RetrievePath()} @ {t.transform.position.Repr()}";
    // }
    //
    // private static string ToRepr(this Component t)
    // {
    //     return
    //         $"Component {t.gameObject.RetrievePath()}/{t.GetType().Name} @ {t.transform.position.Repr()}";
    // }
    //
    // private static string ToRepr(this Transform t)
    // {
    //     return $"Transform {t.gameObject.RetrievePath()}/{t.name} @ {t.position.Repr()}";
    // }
}