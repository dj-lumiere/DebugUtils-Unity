using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Color))]
    [ReprOptions(needsPrefix: true)]
    internal class ColorFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Color)obj;
            return $"R{t.r * 100:F0}% G{t.g * 100:F0}% B{t.b * 100:F0}% A{t.a * 100:F0}%";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Color)obj;
            Color.RGBToHSV(rgbColor: t, H: out var h, S: out var s, V: out var v);
            return new JObject
            {
                [propertyName: "type"] = "Color",
                [propertyName: "kind"] = "struct",
                [propertyName: "r"] = t.r.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "g"] = t.g.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "b"] = t.b.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "a"] = t.a.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "h"] = h.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "s"] = s.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "v"] = v.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }

    [ReprFormatter(typeof(Color32))]
    [ReprOptions(needsPrefix: true)]
    internal class Color32Formatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Color32)obj;
            return $"R{t.r:X2} G{t.g:X2} B{t.b:X2} A{t.a:X2}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Color32)obj;
            Color.RGBToHSV(rgbColor: t, H: out var h, S: out var s, V: out var v);
            var hByte = (byte)(h * 255);
            var sByte = (byte)(s * 255);
            var vByte = (byte)(v * 255);
            return new JObject
            {
                [propertyName: "type"] = "Color32",
                [propertyName: "kind"] = "struct",
                [propertyName: "r"] = t.r.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "g"] = t.g.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "b"] = t.b.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "a"] = t.a.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "h"] =
                    hByte.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "s"] =
                    sByte.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "v"] = vByte.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }
}