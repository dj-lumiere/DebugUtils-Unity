#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.Models;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Unity.Mathematics;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(half))]
    [ReprOptions(needsPrefix: false)]
    internal class UnityHalfFormatter : IReprFormatter, IReprTreeFormatter
    {
        // IEEE 754 binary16 (Half): 1 sign bit + 5 exponent bits + 10 mantissa bits = 16 bits
        private static readonly FloatSpec F16Spec = new(expBitSize: 5, mantissaBitSize: 10,
            mantissaMask: 0x3FF, mantissaMsbMask: 0x200, expMask: 0x1F, expOffset: 15);

        private static FloatInfo AnalyzeHalf(half value)
        {
            var bits = value.value;
            var rawExponent = (int)(bits >> F16Spec.MantissaBitSize & F16Spec.ExpMask);
            var mantissa = bits & F16Spec.MantissaMask;
            return new FloatInfo(spec: F16Spec, bits: bits, realExponent: rawExponent -
                F16Spec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0) - F16Spec.MantissaBitSize, significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1 << F16Spec.MantissaBitSize) + mantissa), typeName: FloatTypeKind.Half);
        }

        public string ToRepr(object obj, ReprContext context)
        {
            var h = (half)obj;
            var info = AnalyzeHalf(value: h);
            return FormatWithCustomString(obj: h, info: info,
                formatString: context.Config.FloatFormatString, culture: context.Config.Culture);
        }

        private static string FormatWithCustomString(half obj, FloatInfo info, string formatString,
            CultureInfo? culture)
        {
            return formatString switch
            {
                _ when info.IsPositiveInfinity => "Infinity",
                _ when info.IsNegativeInfinity => "-Infinity",
                _ when info.IsQuietNaN => $"QuietNaN(0x{info.Mantissa:X3})",
                _ when info.IsSignalingNaN => $"SignalingNaN(0x{info.Mantissa:X3})",
                "EX" => obj.FormatAsExact(info: info),
                "HP" => obj.FormatAsHexPower(info: info),
                _ => obj.ToString(format: formatString, formatProvider: culture)
            };
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            if (context.Depth > 0)
            {
                return obj.Repr(context: context)!.ToJValue();
            }

            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName()
                                             .ToJValue(),
                [propertyName: "kind"] = type.GetTypeKind()
                                             .ToJValue(),
                [propertyName: "value"] = ToRepr(obj: obj, context: context)
                   .ToJValue()
            };
        }
    }
}