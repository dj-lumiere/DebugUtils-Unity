using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using UnityEngine;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Vector2))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector2Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector2)obj;
            return $"{t.x:F2},{t.y:F2}";
        }
    }

    [ReprFormatter(typeof(Vector3))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector3Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector3)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2}";
        }
    }

    [ReprFormatter(typeof(Vector4))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector4Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector4)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2}";
        }
    }

    [ReprFormatter(typeof(Quaternion))]
    [ReprOptions(needsPrefix: true)]
    internal class QuaternionFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Quaternion)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2}";
        }
    }
}