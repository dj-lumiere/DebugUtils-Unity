using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.SceneNavigator;
using Color = UnityEngine.Color;

namespace DebugUtils.Unity.Repr.Formatters
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

    internal class Vector2Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Vector2)obj;
            return $"Vector2({t.x:F2},{t.y:F2})";
        }
    }

    internal class Vector3Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Vector3)obj;
            return $"Vector3({t.x:F2},{t.y:F2},{t.z:F2})";
        }
    }

    internal class Vector4Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Vector4)obj;
            return $"Vector4({t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2})";
        }
    }

    internal class QuaternionFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Quaternion)obj;
            return $"Quaternion({t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2})";
        }
    }

    internal class Color32Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Color32)obj;
            return $"Color32(R{t.r:X2}G{t.g:X2}B{t.b:X2}A{t.a:X2})";
        }
    }

    internal class GameObjectFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.GameObject)obj;
            return
                $"Component {t.gameObject.RetrievePath()}/{t.GetType().Name} @ {t.transform.position.Repr()}";
        }
    }

    internal class TransformFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (UnityEngine.Transform)obj;
            return $"Transform {t.gameObject.RetrievePath()}/{t.name} @ {t.position.Repr()}";
        }
    }
}