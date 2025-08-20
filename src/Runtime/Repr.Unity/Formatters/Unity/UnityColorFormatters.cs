#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using Newtonsoft.Json.Linq;
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
            var colorHex = ColorUtility.ToHtmlStringRGBA(color: t);
            var colorSquare = $"<color=#{colorHex}>■■■</color>";
            return
                $"R{t.r * 100:F0}% G{t.g * 100:F0}% B{t.b * 100:F0}% A{t.a * 100:F0}% {colorSquare}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Color)obj;
            context = context.WithContainerConfig();
            Color.RGBToHSV(rgbColor: t, H: out var h, S: out var s, V: out var v);
            h *= 360;
            s *= 100;
            v *= 100;
            var rInt = (int)(t.r * 255);
            var gInt = (int)(t.g * 255);
            var bInt = (int)(t.b * 255);
            var aInt = (int)(t.a * 255);
            return new JObject
            {
                [propertyName: "type"] = "Color".ToJValue(),
                [propertyName: "kind"] = "struct".ToJValue(),
                [propertyName: "r"] = t.r.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "g"] = t.g.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "b"] = t.b.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "a"] = t.a.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "h"] = h.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "s"] = s.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "v"] = v.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "rgbaForm"] = $"#{rInt:X2}{gInt:X2}{bInt:X2}{aInt:X2}".ToJValue()
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
            var colorHex = ColorUtility.ToHtmlStringRGBA(color: t);
            var colorSquare = $"<color=#{colorHex}>■■■</color>";
            return $"RGBA = #{t.r:X2}{t.g:X2}{t.b:X2}{t.a:X2} {colorSquare}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Color32)obj;
            context = context.WithContainerConfig();
            Color.RGBToHSV(rgbColor: t, H: out var h, S: out var s, V: out var v);
            h *= 360;
            s *= 100;
            v *= 100;
            return new JObject
            {
                [propertyName: "type"] = "Color32".ToJValue(),
                [propertyName: "kind"] = "struct".ToJValue(),
                [propertyName: "r"] = t.r.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "g"] = t.g.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "b"] = t.b.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "a"] = t.a.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "h"] = h.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "s"] = s.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "v"] = v.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "rgbaForm"] = $"#{t.r:X2}{t.g:X2}{t.b:X2}{t.a:X2}".ToJValue()
            };
        }
    }
}