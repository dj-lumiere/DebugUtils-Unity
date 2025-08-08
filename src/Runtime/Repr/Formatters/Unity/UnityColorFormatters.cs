using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using UnityEngine;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Color))]
    [ReprOptions(needsPrefix: true)]
    internal class ColorFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Color)obj;
            return
                $"R{(int)(t.r * 255)} G{(int)(t.g * 255)} B{(int)(t.b * 255)} A{(int)(t.a * 255)}";
        }
    }

    [ReprFormatter(typeof(Color32))]
    [ReprOptions(needsPrefix: true)]
    internal class Color32Formatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Color32)obj;
            return $"R{t.r:X2} G{t.g:X2} B{t.b:X2} A{t.a:X2}";
        }
    }
}