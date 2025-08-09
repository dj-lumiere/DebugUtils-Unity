using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Newtonsoft.Json.Linq;
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
            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Vector2"),
                [propertyName: "kind"] = new JValue(value: "struct"),
                [propertyName: "x"] = t.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "y"] = t.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "magnitude"] =
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    new JObject
                    {
                        [propertyName: "x"] =
                            t.normalized.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "y"] =
                            t.normalized.y.FormatAsJToken(context: context.WithIncrementedDepth())
                    }
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
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    new JObject
                    {
                        [propertyName: "x"] =
                            t.normalized.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "y"] =
                            t.normalized.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "z"] =
                            t.normalized.z.FormatAsJToken(context: context.WithIncrementedDepth())
                    }
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
                    t.magnitude.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "normalized"] =
                    new JObject
                    {
                        [propertyName: "x"] =
                            t.normalized.x.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "y"] =
                            t.normalized.y.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "z"] =
                            t.normalized.z.FormatAsJToken(context: context.WithIncrementedDepth()),
                        [propertyName: "w"] =
                            t.normalized.w.FormatAsJToken(context: context.WithIncrementedDepth())
                    }
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
                [propertyName: "eulerAngles"] =
                    new JObject
                    {
                        [propertyName: "x"] =
                            t.eulerAngles.x.FormatAsJToken(
                                context: context.WithIncrementedDepth()),
                        [propertyName: "y"] =
                            t.eulerAngles.y.FormatAsJToken(
                                context: context.WithIncrementedDepth()),
                        [propertyName: "z"] =
                            t.eulerAngles.z.FormatAsJToken(context: context.WithIncrementedDepth())
                    }
            };
        }
    }
}