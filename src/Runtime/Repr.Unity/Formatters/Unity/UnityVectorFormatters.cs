#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Newtonsoft.Json.Linq;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Vector2))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector2Formatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector2)obj;
            return $"{t.x:F2},{t.y:F2}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Vector2)obj;
            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Vector2"),
                [propertyName: "kind"] = new JValue(value: "struct"),
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }

    [ReprFormatter(typeof(Vector3))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector3Formatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector3)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Vector3)obj;
            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Vector3"),
                [propertyName: "kind"] = new JValue(value: "struct"),
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }

    [ReprFormatter(typeof(Vector4))]
    [ReprOptions(needsPrefix: true)]
    internal class Vector4Formatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Vector4)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Vector4)obj;
            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Vector4"),
                [propertyName: "kind"] = new JValue(value: "struct"),
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "w"] = t.w.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }

    [ReprFormatter(typeof(Quaternion))]
    [ReprOptions(needsPrefix: true)]
    internal class QuaternionFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Quaternion)obj;
            return $"{t.x:F2},{t.y:F2},{t.z:F2},{t.w:F2}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Quaternion)obj;
            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Quaternion"),
                [propertyName: "kind"] = new JValue(value: "struct"),
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "w"] = t.w.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "eulerDegreeX"] =
                    t.eulerAngles.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "eulerDegreeY"] =
                    t.eulerAngles.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "eulerDegreeZ"] =
                    t.eulerAngles.z.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }
}