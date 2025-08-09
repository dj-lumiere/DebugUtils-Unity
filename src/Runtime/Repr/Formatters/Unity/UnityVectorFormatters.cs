using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

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
            return new JObject
            {
                [propertyName: "type"] = "Vector2",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    t.normalized.FormatAsJToken(context: context.WithIncrementedDepth())
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
            return new JObject
            {
                [propertyName: "type"] = "Vector3",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    t.normalized.FormatAsJToken(context: context.WithIncrementedDepth())
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
            return new JObject
            {
                [propertyName: "type"] = "Vector4",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "w"] = t.w.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    t.normalized.FormatAsJToken(context: context.WithIncrementedDepth())
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
            return new JObject
            {
                [propertyName: "type"] = "Quaternion",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "z"] = t.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "w"] = t.w.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "eulerAngles"] =
                    t.eulerAngles.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }
}